using TaskTracker.Client.DTOs.Column;
using TaskTracker.Client.Services.Interfaces;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Pages.BoardDetails;

public class ColumnManager
{
    private readonly IBoardPageService _boardPageService;
    private readonly Func<BoardPageState> _getBoardState;
    private readonly Action<BoardPageState> _setBoardState;

    public ColumnManager(
        IBoardPageService boardPageService,
        Func<BoardPageState> getBoardState,
        Action<BoardPageState> setBoardState)
    {
        _boardPageService = boardPageService;
        _getBoardState = getBoardState;
        _setBoardState = setBoardState;
    }

    public void ShowColumnModal()
    {
        var boardState = _getBoardState();
        boardState.IsColumnModalVisible = true;
        _setBoardState(boardState);
    }

    public Task HideColumnModal()
    {
        var boardState = _getBoardState();
        boardState.IsColumnModalVisible = false;
        _setBoardState(boardState);
        return Task.CompletedTask;
    }

    public async Task HandleColumnCreatedAsync(CreateColumnDto dto)
    {
        var success = await _boardPageService.CreateColumnAsync(dto.BoardId, dto.Title);
        if (success)
        {
            var boardState = await _boardPageService.LoadBoardAsync(dto.BoardId);
            _setBoardState(boardState);
        }

        var currentState = _getBoardState();
        currentState.IsColumnModalVisible = false;
        _setBoardState(currentState);
    }

    public async Task OnColumnDeleteAsync(ColumnDto column)
    {
        var boardState = _getBoardState();
        if (boardState.IsColumnDeleting) return;

        boardState.IsColumnDeleting = true;
        _setBoardState(boardState);

        try
        {
            var success = await _boardPageService.DeleteColumnAsync(column.Id);
            if (success)
            {
                boardState.RemoveColumn(column.Id);
            }
        }
        finally
        {
            boardState.IsColumnDeleting = false;
            _setBoardState(boardState);
        }
    }

    public async Task OnColumnTitleEditingChangedAsync(Guid? columnId)
    {
        var boardState = _getBoardState();
        boardState.EditingColumnId = columnId;
        _setBoardState(boardState);
    }

    public async Task SaveColumnTitleAsync((Guid ColumnId, string NewTitle) data)
    {
        var boardState = _getBoardState();
        boardState.IsColumnTitleSaving = true;
        _setBoardState(boardState);

        try
        {
            var success = await _boardPageService.UpdateColumnTitleAsync(data.ColumnId, data.NewTitle);

            if (success)
            {
                boardState.UpdateColumnTitle(data.ColumnId, data.NewTitle);
                boardState.EditingColumnId = null;
                Console.WriteLine($"Column title updated successfully: {data.NewTitle}");
            }
            else
            {
                Console.WriteLine("Failed to update column title");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating column title: {ex.Message}");
        }
        finally
        {
            boardState.IsColumnTitleSaving = false;
            _setBoardState(boardState);
        }
    }

    public async Task OnColumnEditAsync((Guid columnId, string newTitle) data)
    {
        var success = await _boardPageService.UpdateColumnTitleAsync(data.columnId, data.newTitle);
        if (success)
        {
            var boardState = _getBoardState();
            boardState.UpdateColumnTitle(data.columnId, data.newTitle);
            _setBoardState(boardState);
        }
    }

    public async Task OnColumnMoveAsync(MoveColumnDto model)
    {
        var success = await _boardPageService.MoveColumnAsync(model);
        if (success)
        {
            var boardState = _getBoardState();
            boardState.MoveColumn(model.ColumnId, model.BeforeColumnId);
            _setBoardState(boardState);
        }
    }
}