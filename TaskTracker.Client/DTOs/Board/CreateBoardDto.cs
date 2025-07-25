namespace TaskTracker.Client.DTOs.Board;

public class CreateBoardDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public int UserRole { get; set; } = 1;
}