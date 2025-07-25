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
    private const int MaxTitleLength = 128;

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
        var trimmedTitle = newTitle?.Trim() ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(trimmedTitle) && trimmedTitle.Length <= MaxTitleLength)
        {
            await OnAddCard.InvokeAsync(trimmedTitle);
            await Cancel();
        }
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
        {
            await Save();
        }
        else if (e.Key == "Escape")
        {
            await Cancel();
        }
    }

    private Task HandleInput(ChangeEventArgs e)
    {
        newTitle = e.Value?.ToString() ?? string.Empty;

        if (newTitle.Length > MaxTitleLength)
        {
            newTitle = newTitle.Substring(0, MaxTitleLength);
        }

        StateHasChanged();
        return Task.CompletedTask;
    }
}