namespace TaskTracker.Client.DTOs.Card;

public class UpdateCardDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid ColumnId { get; set; }
    public int RowIndex { get; set; }
    public Guid UpdatedBy { get; set; }
}
