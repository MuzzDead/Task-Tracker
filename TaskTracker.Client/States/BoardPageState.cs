using TaskTracker.Client.DTOs.Board;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;

namespace TaskTracker.Client.States;

public class BoardPageState
{
    public BoardDto? Board { get; set; }
    public List<ColumnDto> Columns { get; set; } = new();
    public Dictionary<Guid, List<CardDto>> CardsByColumn { get; set; } = new();
    public bool IsLoading { get; set; } = true;
    public string? Error { get; set; }
    public bool IsColumnModalVisible { get; set; }
    public bool IsTitleEditing { get; set; }
    public bool IsTitleSaving { get; set; }
    public bool IsColumnDeleting { get; set; }
    public Guid? EditingColumnId { get; set; }
    public bool IsColumnTitleSaving { get; set; }

    public static BoardPageState WithError(string error) => new()
    {
        Error = error,
        IsLoading = false,
        Board = null,
        Columns = new(),
        CardsByColumn = new()
    };

    public static BoardPageState Loading() => new()
    {
        IsLoading = true,
        Error = null
    };

    public void SetBoard(BoardDto board, List<ColumnDto> columns, Dictionary<Guid, List<CardDto>> cardsByColumn)
    {
        Board = board;
        Columns = columns;
        CardsByColumn = cardsByColumn;
        IsLoading = false;
        Error = null;
    }

    public void UpdateBoardTitle(string newTitle)
    {
        if (Board != null)
        {
            Board.Title = newTitle;
        }
    }

    public void UpdateColumnTitle(Guid columnId, string newTitle)
    {
        var column = Columns.FirstOrDefault(c => c.Id == columnId);
        if (column != null)
        {
            column.Title = newTitle;
        }
    }

    public void AddColumn(ColumnDto column)
    {
        Columns.Add(column);
        CardsByColumn[column.Id] = new List<CardDto>();
    }

    public void RemoveColumn(Guid columnId)
    {
        Columns.RemoveAll(c => c.Id == columnId);
        CardsByColumn.Remove(columnId);
    }

    public void UpdateCardsForColumn(Guid columnId, List<CardDto> cards)
    {
        CardsByColumn[columnId] = cards;
    }

    public void RemoveCardFromColumn(Guid cardId)
    {
        foreach (var columnCards in CardsByColumn.Values)
        {
            columnCards.RemoveAll(c => c.Id == cardId);
        }
    }
}