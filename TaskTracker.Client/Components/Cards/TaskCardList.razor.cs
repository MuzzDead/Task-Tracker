using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TaskTracker.Client.DTOs.Card;

namespace TaskTracker.Client.Components.Cards;

public partial class TaskCardList : ComponentBase
{
    [Parameter] public List<CardDto> Cards { get; set; } = new();
    [Parameter] public EventCallback<CardDto> OnCardClick { get; set; }
    [Parameter] public EventCallback<string> OnAddCard { get; set; }

    private bool IsAdding;
    private string newTitle = string.Empty;
    private ElementReference newCardInputRef;

    private async Task Start()
    {
        IsAdding = true;
        newTitle = string.Empty;
        StateHasChanged();

        await Task.Yield();
        await newCardInputRef.FocusAsync();
    }

    private Task Cancel()
    {
        IsAdding = false;
        newTitle = string.Empty;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task Save()
    {
        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            await OnAddCard.InvokeAsync(newTitle.Trim());
            await Cancel();
        }
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            await Save();
        else if (e.Key == "Escape")
            await Cancel();
    }
}
