namespace TaskTracker.Client.DTOs.Column;

public class CreateColumnDto
{
    public string Title { get; set; } = string.Empty;
    public Guid BoardId { get; set; }
}
