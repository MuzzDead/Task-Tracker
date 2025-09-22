namespace TaskTracker.Client.DTOs.Comment;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = default!;
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public ICollection<CommentAttachmentDto> Attachments { get; set; } = new List<CommentAttachmentDto>();
}
