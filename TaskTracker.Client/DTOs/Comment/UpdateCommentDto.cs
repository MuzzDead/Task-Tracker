namespace TaskTracker.Client.DTOs.Comment;

public class UpdateCommentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public Guid UpdatedBy { get; set; }
}
