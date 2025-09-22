namespace TaskTracker.Application.DTOs;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public ICollection<CommentAttachmentDto> Attachments { get; set; } = new List<CommentAttachmentDto>();
}