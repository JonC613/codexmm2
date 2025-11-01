namespace ManualMaster.Api.Models;

public class Manual
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string Content { get; set; } = string.Empty;
    public byte[]? FileData { get; set; }
    public string? FileType { get; set; }
    public string? FileName { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    public int Size { get; set; }
    public string? SourceUrl { get; set; }
    public string? SearchQuery { get; set; }
}
