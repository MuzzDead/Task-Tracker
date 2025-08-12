using TaskTracker.Client.Services.Interfaces;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Pages.BoardDetails;

public class CardStateManager
{
    private readonly ICardModalService _cardModalService;
    private readonly Func<Guid> _getCurrentUserId;
    private readonly IAuthStateService _authStateService;
    private readonly Func<CardModalState> _getCardModalState;
    private readonly Action<CardModalState> _setCardModalState;

    public CardStateManager(ICardModalService cardModalService, 
        Func<Guid> getCurrentUserId, 
        IAuthStateService authStateService, 
        Func<CardModalState> getCardModalState, 
        Action<CardModalState> setCardModalState)
    {
        _cardModalService = cardModalService;
        _getCurrentUserId = getCurrentUserId;
        _authStateService = authStateService;
        _getCardModalState = getCardModalState;
        _setCardModalState = setCardModalState;
    }

    public async Task OnCompleteTaskAsync(Guid cardId, bool isCompleted)
    {
        var cardModalState = _getCardModalState();

        try
        {
            var success = await _cardModalService.UpdateCardStateAsync(cardId, isCompleted);
            if (success)
            {
                var updatedState = await _cardModalService.GetStateByCardAsync(cardId);
                cardModalState.State = updatedState;
                cardModalState.SetCardStates(updatedState);

                cardModalState.IsCompleted = isCompleted;
            }
        }
        finally
        {
            _setCardModalState(cardModalState);
        }
    }
}
