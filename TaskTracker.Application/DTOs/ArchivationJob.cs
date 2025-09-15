namespace TaskTracker.Application.DTOs;

public class ArchivationJob
{
    public string id { get; set; } = Guid.NewGuid().ToString();
    public Guid BoardId { get; set; }
    public string Status { get; set; } = string.Empty; // "Success" or "Failed"
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public string? BlobUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
