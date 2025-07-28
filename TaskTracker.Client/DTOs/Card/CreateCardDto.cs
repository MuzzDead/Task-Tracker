namespace TaskTracker.Client.DTOs.Card;

public class CreateCardDto
{
    public string Title { get; set; } = string.Empty;
    public Guid ColumnId { get; set; }
    public int RowIndex { get; set; } = 0;
    public Guid CreatedBy { get; set; }
}
