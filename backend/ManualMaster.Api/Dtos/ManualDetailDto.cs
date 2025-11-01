namespace ManualMaster.Api.Dtos;

public record ManualDetailDto(
    int Id,
    string Title,
    string Category,
    IReadOnlyCollection<string> Tags,
    string Content,
    DateTime UploadDate,
    string? SourceUrl,
    string? FileName,
    string? FileType,
    int Size,
    string? SearchQuery);
