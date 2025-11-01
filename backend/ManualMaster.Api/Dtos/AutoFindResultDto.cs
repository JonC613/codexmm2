namespace ManualMaster.Api.Dtos;

public record AutoFindResultDto(
    string Title,
    string SourceUrl,
    string ContentPreview,
    string Snippet,
    DateTime RetrievedAt);
