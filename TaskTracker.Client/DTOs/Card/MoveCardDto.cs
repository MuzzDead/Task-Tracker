namespace TaskTracker.Client.DTOs.Card;

public class MoveCardDto
{
    public Guid CardId { get; set; }
    public Guid ColumnId { get; set; }
}
