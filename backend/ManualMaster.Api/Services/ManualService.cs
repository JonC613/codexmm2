using System.Text;
using ManualMaster.Api.Dtos;
using ManualMaster.Api.Models;
using ManualMaster.Api.Services.Search;

namespace ManualMaster.Api.Services;

public class ManualService : IManualService
{
    private const int MaxFileBytes = 25 * 1024 * 1024; // 25 MB limit

    private readonly IManualRepository _repository;
    private readonly IManualSearchService _searchService;
    private readonly PdfTextExtractor _pdfTextExtractor;
    private readonly ILogger<ManualService> _logger;

    public ManualService(
        IManualRepository repository,
        IManualSearchService searchService,
        PdfTextExtractor pdfTextExtractor,
        ILogger<ManualService> logger)
    {
        _repository = repository;
        _searchService = searchService;
        _pdfTextExtractor = pdfTextExtractor;
        _logger = logger;
    }

    public async Task<(IReadOnlyList<ManualSummaryDto> Items, int Total)> GetManualsAsync(ManualQueryParameters query, CancellationToken cancellationToken)
    {
        var normalizedSearch = _searchService.NormalizeSearchTerm(query.Search);
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var manuals = await _repository.GetManualsAsync(query.Category, normalizedSearch, page, pageSize, cancellationToken);
        var total = await _repository.CountManualsAsync(query.Category, normalizedSearch, cancellationToken);

        var items = manuals.Select(MapToSummary).ToList();
        return (items, total);
    }

    public async Task<ManualDetailDto?> GetManualAsync(int id, CancellationToken cancellationToken)
    {
        var manual = await _repository.GetByIdAsync(id, cancellationToken);
        return manual is null ? null : MapToDetail(manual);
    }

    public async Task<(byte[] Data, string FileName, string ContentType)?> GetManualFileAsync(int id, CancellationToken cancellationToken)
    {
        var manual = await _repository.GetByIdAsync(id, cancellationToken);
        if (manual?.FileData is null || manual.FileData.Length == 0)
        {
            return null;
        }

        var fileName = manual.FileName ?? $"manual-{id}.{InferExtension(manual.FileType)}";
        var contentType = string.IsNullOrWhiteSpace(manual.FileType) ? "application/octet-stream" : manual.FileType!;
        return (manual.FileData, fileName, contentType);
    }

    public async Task<ManualDetailDto> CreateManualAsync(ManualCreateRequest request, CancellationToken cancellationToken)
    {
        var manual = new Manual
        {
            Title = request.Title.Trim(),
            Category = request.Category.Trim(),
            Tags = DeduplicateTags(request.Tags),
            Content = request.Content?.Trim() ?? string.Empty,
            SourceUrl = string.IsNullOrWhiteSpace(request.SourceUrl) ? null : request.SourceUrl,
            SearchQuery = string.IsNullOrWhiteSpace(request.SearchQuery) ? null : request.SearchQuery,
            UploadDate = DateTime.UtcNow
        };

        await ApplyFileDataAsync(manual, request.Base64FileData, request.FileName, request.FileType, cancellationToken);

        if (string.IsNullOrWhiteSpace(manual.Content))
        {
            manual.Content = manual.FileData is null
                ? "No content provided."
                : "(Content extracted from uploaded file was empty.)";
        }

        await _repository.AddAsync(manual, cancellationToken);
        return MapToDetail(manual);
    }

    public async Task<ManualDetailDto?> UpdateManualAsync(int id, ManualUpdateRequest request, CancellationToken cancellationToken)
    {
        var manual = await _repository.GetByIdAsync(id, cancellationToken);
        if (manual is null)
        {
            return null;
        }

        manual.Title = request.Title.Trim();
        manual.Category = request.Category.Trim();
        manual.Tags = DeduplicateTags(request.Tags);
        manual.Content = request.Content?.Trim() ?? manual.Content;
        manual.SourceUrl = string.IsNullOrWhiteSpace(request.SourceUrl) ? null : request.SourceUrl;
        manual.SearchQuery = string.IsNullOrWhiteSpace(request.SearchQuery) ? null : request.SearchQuery;

        if (!string.IsNullOrWhiteSpace(request.Base64FileData))
        {
            await ApplyFileDataAsync(manual, request.Base64FileData, request.FileName, request.FileType, cancellationToken);
        }

        await _repository.UpdateAsync(manual, cancellationToken);
        return MapToDetail(manual);
    }

    public async Task<bool> DeleteManualAsync(int id, CancellationToken cancellationToken)
    {
        var manual = await _repository.GetByIdAsync(id, cancellationToken);
        if (manual is null)
        {
            return false;
        }

        await _repository.DeleteAsync(manual, cancellationToken);
        return true;
    }

    public Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken)
        => _repository.GetCategoriesAsync(cancellationToken);

    private async Task ApplyFileDataAsync(Manual manual, string? base64Data, string? fileName, string? fileType, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(base64Data))
        {
            return;
        }

        byte[] bytes;
        var commaIndex = base64Data.IndexOf(',');
        string base64String;
        if (commaIndex >= 0)
        {
            base64String = base64Data.Substring(commaIndex + 1);
        }
        else
        {
            base64String = base64Data;
        }

        try
        {
            bytes = Convert.FromBase64String(base64String);
        }
        catch (FormatException ex)
        {
            _logger.LogWarning(ex, "Failed to decode manual file");
            throw new InvalidOperationException("File data must be valid Base64.");
        }

        if (bytes.Length > MaxFileBytes)
        {
            throw new InvalidOperationException($"File exceeds upload limit of {MaxFileBytes / (1024 * 1024)} MB.");
        }

        manual.FileData = bytes;
        manual.Size = bytes.Length;
        manual.FileName = string.IsNullOrWhiteSpace(fileName) ? null : fileName;
        manual.FileType = string.IsNullOrWhiteSpace(fileType) ? DetectMimeType(bytes, fileName) : fileType;

        if (manual.FileType is not null && manual.FileType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            await using var stream = new MemoryStream(bytes);
            var extracted = await _pdfTextExtractor.TryExtractTextAsync(stream, cancellationToken);
            if (!string.IsNullOrWhiteSpace(extracted))
            {
                manual.Content = extracted;
            }
        }
        else if (manual.FileType is not null && manual.FileType.Equals("text/plain", StringComparison.OrdinalIgnoreCase))
        {
            manual.Content = Encoding.UTF8.GetString(bytes);
        }
    }

    private static ManualSummaryDto MapToSummary(Manual manual) => new(
        manual.Id,
        manual.Title,
        manual.Category,
        manual.Tags,
        manual.UploadDate,
        manual.SourceUrl,
        manual.Size);

    private static ManualDetailDto MapToDetail(Manual manual) => new(
        manual.Id,
        manual.Title,
        manual.Category,
        manual.Tags,
        manual.Content,
        manual.UploadDate,
        manual.SourceUrl,
        manual.FileName,
        manual.FileType,
        manual.Size,
        manual.SearchQuery);

    private static List<string> DeduplicateTags(IEnumerable<string> tags)
    {
        return tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Where(tag => tag.Length > 0)
            .Select(tag => tag.Length > 50 ? tag[..50] : tag)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string InferExtension(string? fileType)
    {
        return fileType switch
        {
            "application/pdf" => "pdf",
            "text/plain" => "txt",
            _ => "bin"
        };
    }

    private static string? DetectMimeType(byte[] bytes, string? fileName)
    {
        if (!string.IsNullOrWhiteSpace(fileName) && fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return "application/pdf";
        }

        if (!string.IsNullOrWhiteSpace(fileName) && fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            return "text/plain";
        }

        // Simple PDF signature check
        if (bytes.Length > 4 && bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46)
        {
            return "application/pdf";
        }

        return "application/octet-stream";
    }
}
