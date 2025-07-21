using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Board;

namespace TaskTracker.Client.Components.Boards;

public partial class BoardCard : ComponentBase
{
    [Parameter, EditorRequired] public BoardDto Board { get; set; } = default!;
    [Parameter] public int TasksCount { get; set; }
    [Parameter] public int MembersCount { get; set; }
    [Parameter] public List<string> Members { get; set; } = new();
    [Parameter] public EventCallback<Guid> OnBoardClick { get; set; }

    private string GetDescription()
      => string.IsNullOrWhiteSpace(Board.Description)
         ? "No description provided"
         : Board.Description;
}
