using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;

namespace TaskTracker.Client.Components.Columns;

public partial class ColumnCard : ComponentBase
{
    [Parameter, EditorRequired] public ColumnDto Column { get; set; } = default!;
    [Parameter] public List<CardDto> Cards { get; set; } = new();
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback<CardDto> OnCardClick { get; set; }
    [Parameter] public EventCallback<(string title, Guid columnId)> OnAddCard { get; set; }

    private async Task HandleAddCard(string title)
    {
        await OnAddCard.InvokeAsync((title, Column.Id));
    }
}
