namespace TaskTracker.Client.DTOs.Column;

public class UpdateColumnDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
}
