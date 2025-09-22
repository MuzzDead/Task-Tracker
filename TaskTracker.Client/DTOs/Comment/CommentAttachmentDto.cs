namespace TaskTracker.Client.DTOs.Comment;

public class CommentAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = default!;
    public string DownloadUrl { get; set; } = string.Empty;
    public long Size { get; set; }
}
