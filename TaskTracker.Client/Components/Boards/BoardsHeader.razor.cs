using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace TaskTracker.Client.Components.Boards;

public partial class BoardsHeader
{
    [Parameter] public string Title { get; set; } = "My Boards";
    [Parameter] public bool ShowSearch { get; set; } = true;
    [Parameter] public string SearchTerm { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> OnSearchClick { get; set; }
    [Parameter] public bool IsSearching { get; set; } = false;
    [Parameter] public EventCallback OnCreateClick { get; set; }
    [Parameter] public string ButtonText { get; set; } = "Add Board";

    [Parameter] public bool EnableTitleEdit { get; set; } = false;
    [Parameter] public bool IsTitleEditing { get; set; } = false;
    [Parameter] public EventCallback<bool> OnTitleEditingChanged { get; set; }
    [Parameter] public bool IsTitleSaving { get; set; } = false;
    [Parameter] public EventCallback<string> OnTitleSave { get; set; }
    [Parameter] public bool ShowMembersButton { get; set; } = false;
    [Parameter] public EventCallback OnOpenMembers { get; set; }
    [Parameter] public EventCallback OnArchiveBoard { get; set; }

    private async Task OnSearchButtonClick()
    {
        await OnSearchClick.InvokeAsync(SearchTerm);
    }

    private async Task OnSearchKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await OnSearchClick.InvokeAsync(SearchTerm);
        }
    }

    private async Task OpenMembersDrawer()
    {
        if (OnOpenMembers.HasDelegate)
            await OnOpenMembers.InvokeAsync();
    }

    private async Task ArchiveBoard()
    {
        await OnArchiveBoard.InvokeAsync();
    }
}