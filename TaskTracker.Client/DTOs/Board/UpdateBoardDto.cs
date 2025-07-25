namespace TaskTracker.Client.DTOs.Board;

public class UpdateBoardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid UpdatedBy { get; set; }
}