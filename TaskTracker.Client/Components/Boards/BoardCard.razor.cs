using AntDesign;
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

    private string GetRandomColor(int index)
    {
        var colors = new[] { "#f56565", "#ed8936", "#ecc94b", "#48bb78", "#38b2ac", "#4299e1", "#9f7aea", "#ed64a6" };
        return colors[index % colors.Length];
    }

    private string GetUserInitials(int index)
    {
        var initials = new[] { "AB", "CD", "EF", "GH", "IJ" };
        return initials[index % initials.Length];
    }

    private BadgeStatus GetActivityStatus()
    {
        var daysSinceCreated = (DateTime.Now - Board.CreatedAt).Days;
        return daysSinceCreated switch
        {
            <= 1 => BadgeStatus.Success,
            <= 7 => BadgeStatus.Processing,
            <= 30 => BadgeStatus.Warning,
            _ => BadgeStatus.Default
        };
    }

    private string GetLastActivityText()
    {
        var daysSinceCreated = (DateTime.Now - Board.CreatedAt).Days;
        return daysSinceCreated switch
        {
            0 => "Today",
            1 => "Yesterday",
            <= 7 => $"{daysSinceCreated} days ago",
            <= 30 => $"{daysSinceCreated} days ago",
            _ => "Long ago"
        };
    }
}
