namespace TaskTracker.Client.DTOs.Card;

public class CreateCardDto
{
    public string Title { get; set; } = string.Empty;
    public Guid ColumnId { get; set; }
}
