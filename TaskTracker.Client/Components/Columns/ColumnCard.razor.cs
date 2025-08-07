using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;

namespace TaskTracker.Client.Components.Columns;

public partial class ColumnCard : ComponentBase
{
    [Parameter] public ColumnDto Column { get; set; } = default!;
    [Parameter] public List<CardDto>? Cards { get; set; }
    [Parameter] public bool IsLoading { get; set; }

    [Parameter] public bool IsTitleEditing { get; set; }
    [Parameter] public EventCallback<bool> OnTitleEditingChanged { get; set; }
    [Parameter] public bool IsTitleSaving { get; set; }
    [Parameter] public EventCallback<(Guid ColumnId, string NewTitle)> OnTitleSave { get; set; }

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
        await OnTitleEditingChanged.InvokeAsync(true);
    }

    private async Task HandleDelete()
    {
        await OnDeleteColumn.InvokeAsync(Column);
    }

    private async Task HandleTitleSave(string newTitle)
    {
        await OnTitleSave.InvokeAsync((Column.Id, newTitle));
    }
}