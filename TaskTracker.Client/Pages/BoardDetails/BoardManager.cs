using Microsoft.AspNetCore.Components;
using TaskTracker.Client.Services.Interfaces;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Pages.BoardDetails;

public class BoardManager
{
    private readonly IBoardPageService _boardPageService;
    private readonly NavigationManager _navigationManager;
    private readonly Func<Guid> _getCurrentUserId;
    private readonly Func<BoardPageState> _getBoardState;
    private readonly Action<BoardPageState> _setBoardState;

    public BoardManager(
        IBoardPageService boardPageService,
        NavigationManager navigationManager,
        Func<Guid> getCurrentUserId,
        Func<BoardPageState> getBoardState,
        Action<BoardPageState> setBoardState)
    {
        _boardPageService = boardPageService;
        _navigationManager = navigationManager;
        _getCurrentUserId = getCurrentUserId;
        _getBoardState = getBoardState;
        _setBoardState = setBoardState;
    }

    public async Task LoadBoardDataAsync(Guid boardId)
    {
        var loadingState = BoardPageState.Loading();
        _setBoardState(loadingState);

        var boardState = await _boardPageService.LoadBoardAsync(boardId);
        _setBoardState(boardState);
    }

    public async Task SaveBoardTitleAsync(Guid boardId, string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            return;

        var currentUserId = _getCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            Console.Error.WriteLine("User not authenticated");
            return;
        }

        var boardState = _getBoardState();
        boardState.IsTitleSaving = true;
        _setBoardState(boardState);

        try
        {
            var success = await _boardPageService.UpdateBoardTitleAsync(boardId, newTitle, currentUserId);
            if (success)
            {
                boardState.UpdateBoardTitle(newTitle);
                boardState.IsTitleEditing = false;
                _setBoardState(boardState);
            }
        }
        finally
        {
            boardState.IsTitleSaving = false;
            _setBoardState(boardState);
        }
    }

    public async Task ArchiveBoardAsync(Guid boardId)
    {
        var success = await _boardPageService.ArchiveBoardAsync(boardId);
        if (success)
        {
            _navigationManager.NavigateTo($"/boards");
        }
    }

    public Task OnTitleEditingChanged(bool isEditing)
    {
        var boardState = _getBoardState();
        boardState.IsTitleEditing = isEditing;
        _setBoardState(boardState);
        return Task.CompletedTask;
    }
}