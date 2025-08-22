using Refit;
using TaskTracker.Client.DTOs.Board;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;
using TaskTracker.Client.Services.Interfaces;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Services;

public class BoardPageService : IBoardPageService
{
    private readonly IBoardService _boardService;
    private readonly IColumnService _columnService;
    private readonly ICardService _cardService;

    public BoardPageService(
        IBoardService boardService,
        IColumnService columnService,
        ICardService cardService)
    {
        _boardService = boardService;
        _columnService = columnService;
        _cardService = cardService;
    }

    public async Task<BoardPageState> LoadBoardAsync(Guid boardId)
    {
        try
        {
            var board = await _boardService.GetByIdAsync(boardId);
            var columns = (await _columnService.GetByBoardIdAsync(boardId)).ToList();

            var cardTasks = columns.Select(async column => new
            {
                ColumnId = column.Id,
                Cards = (await _cardService.GetByColumnIdAsync(column.Id)).ToList()
            });

            var cardResults = await Task.WhenAll(cardTasks);
            var cardsByColumn = cardResults.ToDictionary(r => r.ColumnId, r => r.Cards);

            var state = new BoardPageState();
            state.SetBoard(board, columns, cardsByColumn);
            return state;
        }
        catch (ApiException apiEx)
        {
            return BoardPageState.WithError($"API Error {apiEx.StatusCode}: {apiEx.Content}");
        }
        catch (Exception ex)
        {
            return BoardPageState.WithError($"Error loading board: {ex.Message}");
        }
    }

    public async Task<bool> UpdateBoardTitleAsync(Guid boardId, string title, Guid userId)
    {
        try
        {
            var currentBoard = await _boardService.GetByIdAsync(boardId);

            var updateDto = new UpdateBoardDto
            {
                Id = boardId,
                Title = title,
                Description = currentBoard.Description,
                UpdatedBy = userId
            };

            await _boardService.UpdateAsync(boardId, updateDto);
            return true;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to update board title: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating board title: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateColumnTitleAsync(Guid columnId, string title)
    {
        try
        {
            var currentColumn = await _columnService.GetByIdAsync(columnId);
            var updateDto = new UpdateColumnDto
            {
                Id = columnId,
                Title = title,
                ColumnIndex = currentColumn.ColumnIndex
            };
            await _columnService.UpdateAsync(columnId, updateDto);
            return true;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to update column title: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating column title: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CreateColumnAsync(Guid boardId, string title)
    {
        try
        {
            var currentColumns = await _columnService.GetByBoardIdAsync(boardId);
            var nextIndex = currentColumns.Any() ? currentColumns.Max(c => c.ColumnIndex) + 1 : 0;

            var createDto = new CreateColumnDto
            {
                Title = title,
                BoardId = boardId,
                ColumnIndex = nextIndex
            };

            var newId = await _columnService.CreateAsync(createDto);
            return newId != Guid.Empty;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to create column: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating column: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteColumnAsync(Guid columnId)
    {
        try
        {
            await _columnService.DeleteAsync(columnId);
            return true;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to delete column: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting column: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CreateCardAsync(Guid columnId, string title, Guid userId)
    {
        try
        {
            var createDto = new CreateCardDto
            {
                Title = title,
                ColumnId = columnId,
                CreatedBy = userId
            };

            var cardId = await _cardService.CreateAsync(createDto);
            return cardId != Guid.Empty;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to create card: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating card: {ex.Message}");
            return false;
        }
    }

    public async Task<List<CardDto>> ReloadCardsForColumnAsync(Guid columnId)
    {
        try
        {
            var cards = await _cardService.GetByColumnIdAsync(columnId);
            return cards.ToList();
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to reload cards: {apiEx.StatusCode}: {apiEx.Content}");
            return new List<CardDto>();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error reloading cards: {ex.Message}");
            return new List<CardDto>();
        }
    }

    public async Task<bool> ArchiveBoardAsync(Guid boardId)
    {
        try
        {
            await _boardService.ArchiveAsync(boardId);
            return true;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to archive board: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error archiving board: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> MoveColumnAsync(MoveColumnDto model)
    {
        try
        {
            await _columnService.MoveAsync(model.ColumnId, model);
            return true;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to move column: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error moving column: {ex.Message}");
            return false;
        }
    }
}