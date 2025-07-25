using Microsoft.AspNetCore.Components;

namespace TaskTracker.Client.Components.Boards;

public partial class BoardsHeader
{
    [Parameter] public string Title { get; set; } = "My Boards";
    [Parameter] public bool ShowSearch { get; set; } = true;
    [Parameter] public string SearchTerm { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> OnSearchChangedValue { get; set; }
    [Parameter] public EventCallback OnCreateClick { get; set; }
    [Parameter] public string ButtonText { get; set; } = "Add Board";

    [Parameter] public bool EnableTitleEdit { get; set; } = false;
    [Parameter] public bool IsTitleEditing { get; set; } = false;
    [Parameter] public EventCallback<bool> OnTitleEditingChanged { get; set; }
    [Parameter] public bool IsTitleSaving { get; set; } = false;
    [Parameter] public EventCallback<string> OnTitleSave { get; set; }
}