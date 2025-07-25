﻿using AntDesign;
using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;

namespace TaskTracker.Client.Components.Columns;

public partial class ColumnList : ComponentBase
{
    [Parameter] public List<ColumnDto>? Columns { get; set; }
    [Parameter] public Dictionary<Guid, List<CardDto>>? CardsByColumn { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback<string> OnAddColumn { get; set; }
    [Parameter] public EventCallback<(string title, Guid columnId)> OnAddCard { get; set; }
    [Parameter] public EventCallback<CardDto> OnCardClick { get; set; }
    [Parameter] public EventCallback<ColumnDto> OnEditColumn { get; set; }
    [Parameter] public EventCallback<ColumnDto> OnDeleteColumn { get; set; }

    private List<CardDto> GetCards(Guid columnId)
    {
        return CardsByColumn?.ContainsKey(columnId) == true
            ? CardsByColumn[columnId]
            : new List<CardDto>();
    }

    private async Task HandleAddCard((Guid ColumnId, string Title) data)
    {
        await OnAddCard.InvokeAsync((data.Title, data.ColumnId));
    }
}
