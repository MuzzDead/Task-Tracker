namespace TaskTracker.Client.DTOs.Column;

public class ColumnMoveFormModel
{
    public string ColumnName { get; set; } = string.Empty;
    public Guid BeforeColumnId { get; set; }
}
