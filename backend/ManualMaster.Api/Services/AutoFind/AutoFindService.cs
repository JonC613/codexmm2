using System.Text;
using ManualMaster.Api.Dtos;

namespace ManualMaster.Api.Services.AutoFind;

public class AutoFindService : IAutoFindService
{
    private readonly ILogger<AutoFindService> _logger;

    public AutoFindService(ILogger<AutoFindService> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<AutoFindResultDto>> SearchAsync(AutoFindRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ProductName))
        {
            return Task.FromResult<IReadOnlyList<AutoFindResultDto>>(Array.Empty<AutoFindResultDto>());
        }

        var normalizedName = request.ProductName.Trim();
        var normalizedModel = request.ModelNumber?.Trim();
        var slug = ToSlug(normalizedName, normalizedModel);

        // Stubbed data so the UI has something to render without performing live scraping.
        var now = DateTime.UtcNow;
        var results = new List<AutoFindResultDto>
        {
            new(
                Title: $"{normalizedName} Quick Reference Guide",
                SourceUrl: $"https://manuals.example.com/{slug}/quick-reference.pdf",
                ContentPreview: "Step-by-step instructions and troubleshooting tips in a concise format.",
                Snippet: $"Generated query for '{normalizedName}'{(normalizedModel is null ? string.Empty : $" model '{normalizedModel}'")}.",
                RetrievedAt: now),
            new(
                Title: $"{normalizedName} Full User Manual",
                SourceUrl: $"https://manuals.example.com/{slug}/user-manual.pdf",
                ContentPreview: "Comprehensive guide covering setup, maintenance, and warranty information.",
                Snippet: "This is placeholder data. Integrate with a search provider or manual repository for production.",
                RetrievedAt: now)
        };

        _logger.LogInformation("Auto-find invoked for {Product} {Model}", normalizedName, normalizedModel);
        return Task.FromResult<IReadOnlyList<AutoFindResultDto>>(results);
    }

    private static string ToSlug(string name, string? model)
    {
        var builder = new StringBuilder(name.ToLowerInvariant());
        if (!string.IsNullOrWhiteSpace(model))
        {
            builder.Append('-');
            builder.Append(model.ToLowerInvariant());
        }

        return new string(builder
            .ToString()
            .Select(c => char.IsLetterOrDigit(c) ? c : '-')
            .ToArray())
            .Replace("--", "-")
            .Trim('-');
    }
}
