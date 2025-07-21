using AntDesign;
using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;

namespace TaskTracker.Client.Components.Columns;

public partial class ColumnList : ComponentBase
{
    [Parameter, EditorRequired]
    public List<ColumnDto> Columns { get; set; } = new();

    [Parameter, EditorRequired]
    public Dictionary<Guid, List<CardDto>> CardsByColumn { get; set; } = new();

    [Parameter]
    public bool IsLoading { get; set; }

    [Parameter]
    public EventCallback<CardDto> OnCardClick { get; set; }

    [Parameter]
    public EventCallback<(string title, Guid columnId)> OnAddCard { get; set; }

    [Parameter]
    public EventCallback<(CardDto card, Guid columnId)> OnCardDrop { get; set; }

    [Parameter]
    public EventCallback<string> OnAddColumn { get; set; }

    private List<CardDto> GetCards(Guid columnId)
      => CardsByColumn.TryGetValue(columnId, out var list) ? list : new();
}
