using AntDesign;
using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Board;

namespace TaskTracker.Client.Components.Boards;

public partial class BoardList : ComponentBase
{
    [Parameter, EditorRequired]
    public List<BoardDto> CurrentPageBoards { get; set; } = new();

    [Parameter]
    public bool IsLoading { get; set; }

    [Parameter]
    public int CurrentPage { get; set; }

    [Parameter]
    public int PageSize { get; set; }

    [Parameter]
    public int TotalBoards { get; set; }

    [Parameter]
    public EventCallback<Guid> OnBoardClick { get; set; }

    [Parameter]
    public EventCallback<PaginationEventArgs> OnPageChange { get; set; }

    [Parameter]
    public Dictionary<Guid, int> TasksCounts { get; set; } = new();

    [Parameter]
    public Dictionary<Guid, int> MembersCounts { get; set; } = new();

    [Parameter]
    public Dictionary<Guid, List<string>> BoardMembers { get; set; } = new();

    private int GetBoardTasksCount(Guid boardId) =>
      TasksCounts.TryGetValue(boardId, out var count) ? count : 0;

    private int GetBoardMembersCount(Guid boardId) =>
      MembersCounts.TryGetValue(boardId, out var count) ? count : 0;

    private List<string> GetBoardMembers(Guid boardId) =>
      BoardMembers.TryGetValue(boardId, out var members)
        ? members
        : new List<string>();
}
