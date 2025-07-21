using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TaskTracker.Client.DTOs.Card;

namespace TaskTracker.Client.Components.Cards;

public partial class TaskCard : ComponentBase
{
    [Parameter, EditorRequired] public CardDto Card { get; set; } = default!;
    [Parameter] public EventCallback<CardDto> OnCardClick { get; set; }
    [Parameter] public EventCallback<(CardDto card, DragEventArgs args)> OnDragStart { get; set; }
}
