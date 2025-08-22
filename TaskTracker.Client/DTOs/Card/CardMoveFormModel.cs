namespace TaskTracker.Client.DTOs.Card;

public class CardMoveFormModel
{
    public string CardName { get; set; } = string.Empty;
    public Guid TargetColumnId { get; set; }
}
