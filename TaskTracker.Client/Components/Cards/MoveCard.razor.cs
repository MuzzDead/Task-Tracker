using AntDesign;
using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;
using TaskTracker.Client.Services;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Components.Cards;

public partial class MoveCard
{
    [Parameter] public CardDto? Card { get; set; }
    [Parameter] public ColumnDto? CurrentColumn { get; set; }
    [Parameter] public List<ColumnDto>? BoardColumns { get; set; }
    [Parameter] public EventCallback<MoveCardDto> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    [Inject] private ICardService CardService { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    private bool _visible;
    private bool IsSaving { get; set; }
    private bool IsPositionUpdating { get; set; }
    private CardMoveFormModel _formModel = new();
    private Guid _initialTargetColumnId;

    public void Open(CardDto card, ColumnDto currentColumn, List<ColumnDto> boardColumns)
    {
        Card = card;
        CurrentColumn = currentColumn;
        BoardColumns = boardColumns.OrderBy(c => c.ColumnIndex).ToList();
        InitializeFromCard();
        _visible = true;
        StateHasChanged();
    }

    public void Close()
    {
        _visible = false;
        StateHasChanged();
    }

    private bool IsDirty =>
        _formModel.TargetColumnId != _initialTargetColumnId;

    private void InitializeFromCard()
    {
        if (Card is null) return;

        _formModel.CardName = Card.Title;
        _formModel.TargetColumnId = Guid.Empty;
        _initialTargetColumnId = Guid.Empty;
    }

    private void OnTargetColumnChanged(Guid newTargetColumnId)
    {
        _formModel.TargetColumnId = newTargetColumnId;
    }

    private async Task HandleSave()
    {
        if (Card is null || _formModel.TargetColumnId == Guid.Empty)
            return;

        IsSaving = true;
        try
        {
            var moveDto = new MoveCardDto
            {
                CardId = Card.Id,
                ColumnId = _formModel.TargetColumnId
            };

            await CardService.MoveAsync(Card.Id, moveDto);

            await OnSave.InvokeAsync(moveDto);

            _initialTargetColumnId = _formModel.TargetColumnId;

            Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to move card: {ex.Message}");
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
