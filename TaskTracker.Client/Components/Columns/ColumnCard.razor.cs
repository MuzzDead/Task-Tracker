using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;

namespace TaskTracker.Client.Components.Columns;

public partial class ColumnCard : ComponentBase
{
    [Parameter] public ColumnDto Column { get; set; } = default!;
    [Parameter] public List<CardDto>? Cards { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback<CardDto> OnCardClick { get; set; }
    [Parameter] public EventCallback<(Guid ColumnId, string Title)> OnAddCard { get; set; }
    [Parameter] public EventCallback<ColumnDto> OnEditColumn { get; set; }
    [Parameter] public EventCallback<ColumnDto> OnDeleteColumn { get; set; }

    private async Task HandleAddCard(string title)
    {
        await OnAddCard.InvokeAsync((Column.Id, title));
    }

    private async Task HandleEdit()
    {
        await OnEditColumn.InvokeAsync(Column);
    }

    private async Task HandleDelete()
    {
        await OnDeleteColumn.InvokeAsync(Column);
    }
}