namespace TaskTracker.Application.DTOs;

public class ColumnDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid BoardId { get; set; }
    public int ColumnIndex {  get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}