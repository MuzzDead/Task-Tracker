using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.State;

namespace TaskTracker.Client.Components.Cards;

public partial class CardStateEditModal
{

    [Parameter] public CardDto? Card { get; set; }
    [Parameter] public StateDto? CardState { get; set; }
    [Parameter] public EventCallback<(Guid cardId, Priority priority, DateTimeOffset? deadline)> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private bool _visible;
    private bool IsSaving { get; set; }
    private bool IsPriorityUpdating { get; set; }
    private StateFormModel _formModel = new();
    private Priority _initialPriority;
    private DateTimeOffset? _initialDeadline;

    public void Open(CardDto card, StateDto? state)
    {
        Card = card;
        CardState = state;
        InitializeFromState();
        _visible = true;
        StateHasChanged();
    }

    public void Close()
    {
        _visible = false;
        StateHasChanged();
    }

    private bool IsDirty =>
        _formModel.Priority != _initialPriority ||
        !Nullable.Equals(_formModel.Deadline, _initialDeadline);

    private void InitializeFromState()
    {
        _formModel.Priority = CardState?.Priority ?? Priority.Low;
        _formModel.Deadline = CardState?.Deadline;
        _initialPriority = _formModel.Priority;
        _initialDeadline = _formModel.Deadline;
    }

    private void OnPriorityChanged(Priority newPriority)
    {
        _formModel.Priority = newPriority;
    }

    private async Task HandleSave()
    {
        if (Card is null) return;

        IsSaving = true;
        try
        {
            await OnSave.InvokeAsync((Card.Id, _formModel.Priority, _formModel.Deadline));
            _initialPriority = _formModel.Priority;
            _initialDeadline = _formModel.Deadline;
            Close();
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task HandleCancel()
    {
        Close();
        await OnCancel.InvokeAsync();
    }

    private async Task HandleBackdropClick()
    {
        await HandleCancel();
    }
}