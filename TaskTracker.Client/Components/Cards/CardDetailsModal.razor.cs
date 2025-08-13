using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Comment;
using TaskTracker.Client.DTOs.State;
using TaskTracker.Client.DTOs.User;

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
    [Parameter] public StateDto? State { get; set; }
    [Parameter] public UserDto? AssignedUser { get; set; }
    [Parameter] public bool IsAssigneeLoading { get; set; }
    [Parameter] public bool IsRemovingAssignment { get; set; }
    [Parameter] public bool IsCurrentUserAssigned { get; set; }
    [Parameter] public EventCallback<Guid> OnRemoveAssignment { get; set; }
    [Parameter] public EventCallback OnOpenAssignModal { get; set; }

    [Parameter] public EventCallback<(Guid cardId, Priority priority, DateTimeOffset? deadline)> OnStateEdit { get; set; }

    private CardStateEditModal StateEditModal = default!;

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

    private async Task HandleRemoveAssignment()
    {
        if (Card != null)
        {
            await OnRemoveAssignment.InvokeAsync(Card.Id);
        }
    }

    private async Task HandleOpenAssignModal()
    {
        await OnOpenAssignModal.InvokeAsync();
    }

    private void HandleOpenStateEditModal()
    {
        if (Card != null)
        {
            StateEditModal.Open(Card, State);
        }
    }

    private async Task HandleStateEditCancel()
    {
        StateEditModal.Close();
    }

    private string GetPriorityText()
    {
        if (State == null) return "No priority";
        return State.Priority switch
        {
            Priority.Low => "Low priority",
            Priority.Medium => "Medium priority",
            Priority.High => "High priority",
            Priority.Critical => "Critical priority",
            _ => "Unknown priority"
        };
    }

    private string GetPriorityColor()
    {
        if (State == null) return "default";
        return State.Priority switch
        {
            Priority.Low => "green-inverse",
            Priority.Medium => "yellow-inverse",
            Priority.High => "orange-inverse",
            Priority.Critical => "red-inverse",
            _ => "default"
        };
    }

    private DateTimeOffset? GetDeadline()
    {
        return State?.Deadline;
    }
}