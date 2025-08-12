using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Comment;

namespace TaskTracker.Client.Components.Cards;

public partial class CardDetailsModal : ComponentBase
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
    [Parameter] public CardDto? Card { get; set; }
    [Parameter] public List<CommentDto> Comments { get; set; } = new();

    [Parameter] public bool IsTitleEditing { get; set; }
    [Parameter] public bool IsTitleSaving { get; set; }
    [Parameter] public EventCallback<bool> OnTitleEditingChanged { get; set; }
    [Parameter] public EventCallback<string> OnTitleSave { get; set; }

    [Parameter] public bool IsCommentsLoading { get; set; }
    [Parameter] public bool IsCommentSubmitting { get; set; }
    [Parameter] public EventCallback<string> OnCommentSubmit { get; set; }
    [Parameter] public EventCallback<(Guid commentId, string newContent)> OnCommentEdit { get; set; }
    [Parameter] public EventCallback<Guid> OnCommentDelete { get; set; }

    [Parameter] public bool IsDeleting { get; set; }
    [Parameter] public EventCallback<Guid> OnCardDelete { get; set; }

    [Parameter] public bool IsTaskCompleted { get; set; }
    [Parameter] public EventCallback<(Guid cardId, bool isCompleted)> OnTaskComplete { get; set; }

    private async Task HandleDelete()
    {
        if (Card != null)
        {
            await OnCardDelete.InvokeAsync(Card.Id);
        }
    }

    private async Task HandleClose()
    {
        await IsVisibleChanged.InvokeAsync(false);
    }

    private async Task HandleBackdropClick()
    {
        await HandleClose();
    }

    private async Task HandleComplete()
    {
        if (Card == null)
            return;

        await OnTaskComplete.InvokeAsync((Card.Id, !IsTaskCompleted));
    }
}
