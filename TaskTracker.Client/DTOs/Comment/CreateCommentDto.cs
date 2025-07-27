namespace TaskTracker.Client.DTOs.Comment;

public class CreateCommentDto
{
    public string Text { get; set; }
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
}
