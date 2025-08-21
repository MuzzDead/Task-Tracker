namespace TaskTracker.Client.DTOs.Column;

public class MoveColumnDto
{
    public Guid ColumnId { get; set; }
    public Guid BeforeColumnId { get; set; }
}
