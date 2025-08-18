using TaskTracker.Client.DTOs.Member;
using TaskTracker.Client.DTOs.State;
using TaskTracker.Client.Services.Interfaces;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Pages.BoardDetails;

public class CardStateManager
{
    private readonly ICardModalService _cardModalService;
    private readonly IBoardRoleService _boardRoleService;
    private readonly Func<Guid> _getCurrentUserId;
    private readonly IAuthStateService _authStateService;
    private readonly Func<CardModalState> _getCardModalState;
    private readonly Action<CardModalState> _setCardModalState;
    private readonly Guid _boardId;

    public CardStateManager(
        ICardModalService cardModalService,
        Func<Guid> getCurrentUserId,
        IAuthStateService authStateService,
        Func<CardModalState> getCardModalState,
        Action<CardModalState> setCardModalState,
        IBoardRoleService boardRoleService,
        Guid boardId)
    {
        _cardModalService = cardModalService;
        _getCurrentUserId = getCurrentUserId;
        _authStateService = authStateService;
        _getCardModalState = getCardModalState;
        _setCardModalState = setCardModalState;
        _boardRoleService = boardRoleService;
        _boardId = boardId;
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

    public async Task OnStateEditAsync(Guid cardId, Priority? priority, DateTimeOffset? dateLine)
    {
        var cardModalState = _getCardModalState();

        try
        {
            var currentUserId = _getCurrentUserId();
            var success = await _cardModalService.UpdateCardStateFieldsAsync(cardId, priority, dateLine, currentUserId);
            if (success)
            {
                var updatedState = await _cardModalService.GetStateByCardAsync(cardId);
                cardModalState.State = updatedState;
                cardModalState.SetCardStates(updatedState);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating card state: {ex.Message}");
        }
        finally
        {
            _setCardModalState(cardModalState);
        }
    }

    public async Task LoadAssignedUserAsync(Guid cardId, Guid? assigneeId)
    {
        var cardModalState = _getCardModalState();
        cardModalState.IsAssigneeLoading = true;
        _setCardModalState(cardModalState);

        try
        {
            if (assigneeId.HasValue)
            {
                var assignedUser = await _cardModalService.GetAssignedUserAsync(assigneeId.Value);
                if (assignedUser != null)
                {
                    var isCurrentUser = assigneeId.Value == _getCurrentUserId();
                    cardModalState.SetAssignedUser(assignedUser, isCurrentUser);
                }
                else
                {
                    cardModalState.SetAssignedUser(null);
                }
            }
            else
            {
                cardModalState.SetAssignedUser(null);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading assigned user: {ex.Message}");
            cardModalState.SetAssignedUser(null);
        }
        finally
        {
            cardModalState.IsAssigneeLoading = false;
            _setCardModalState(cardModalState);
        }
    }

    public async Task RemoveAssignmentAsync(Guid cardId)
    {
        var cardModalState = _getCardModalState();
        cardModalState.IsRemovingAssignment = true;
        _setCardModalState(cardModalState);

        try
        {
            var success = await _cardModalService.RemoveAssignmentAsync(cardId);
            if (success)
            {
                cardModalState.RemoveAssignedUser();

                var updatedState = await _cardModalService.GetStateByCardAsync(cardId);
                cardModalState.SetCardStates(updatedState);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error removing assignment: {ex.Message}");
        }
        finally
        {
            cardModalState.IsRemovingAssignment = false;
            _setCardModalState(cardModalState);
        }
    }

    public async Task AssignUserAsync(Guid cardId, Guid userId)
    {
        var cardModalState = _getCardModalState();
        cardModalState.IsAssigningUser = true;
        _setCardModalState(cardModalState);

        try
        {
            var success = await _cardModalService.AssignUserAsync(cardId, userId);
            if (success)
            {
                await LoadAssignedUserAsync(cardId, userId);

                var updatedState = await _cardModalService.GetStateByCardAsync(cardId);
                cardModalState.SetCardStates(updatedState);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error assigning user: {ex.Message}");
        }
        finally
        {
            cardModalState.IsAssigningUser = false;
            _setCardModalState(cardModalState);
        }
    }

    public async Task OpenAssignModalAsync()
    {
        var cardModalState = _getCardModalState();
        cardModalState.IsAssignModalVisible = true;
        cardModalState.IsMembersLoading = true;
        _setCardModalState(cardModalState);

        try
        {
            Console.WriteLine($"Loading members for board: {_boardId}");
            var members = await _boardRoleService.GetMemberByBoardIdAsync(_boardId);
            var membersList = members.ToList();
            Console.WriteLine($"Loaded {membersList.Count} members");

            cardModalState.SetBoardMembers(membersList);
            Console.WriteLine($"BoardMembers count in state: {cardModalState.BoardMembers.Count}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading board members: {ex.Message}");
            cardModalState.SetBoardMembers(new List<MemberDto>());
        }
        finally
        {
            _setCardModalState(cardModalState);
        }
    }

    public void CloseAssignModal()
    {
        var cardModalState = _getCardModalState();
        cardModalState.IsAssignModalVisible = false;
        _setCardModalState(cardModalState);
    }
}