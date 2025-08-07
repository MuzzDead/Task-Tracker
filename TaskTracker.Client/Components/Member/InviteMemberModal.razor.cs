using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Member;
using TaskTracker.Client.DTOs.User;
namespace TaskTracker.Client.Components.Member;
public partial class InviteMemberModal : ComponentBase
{
    [Parameter] public bool Visible { get; set; }
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
    [Parameter] public Guid BoardId { get; set; }
    
    [Parameter] public InviteStep CurrentStep { get; set; }
    [Parameter] public string SearchEmail { get; set; } = "";
    [Parameter] public EventCallback<string> SearchEmailChanged { get; set; }
    [Parameter] public string SearchError { get; set; } = "";
    [Parameter] public UserDto? FoundUser { get; set; }
    [Parameter] public UserRole SelectedRole { get; set; } = UserRole.Member;
    [Parameter] public EventCallback<UserRole> SelectedRoleChanged { get; set; }
    [Parameter] public bool IsSearching { get; set; }
    [Parameter] public bool IsSendingInvite { get; set; }
    
    [Parameter] public EventCallback OnUserSearch { get; set; }
    [Parameter] public EventCallback OnSendInvite { get; set; }
    [Parameter] public EventCallback OnBackToSearch { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    
    private async Task HandleSearch()
    {
        await OnUserSearch.InvokeAsync();
    }

    private async Task HandleSendInvite()
    {
        await OnSendInvite.InvokeAsync();
    }

    private async Task OnSelectedRoleChanged(UserRole newRole)
    {
        SelectedRole = newRole;
        await SelectedRoleChanged.InvokeAsync(newRole);
    }

    private async Task BackToSearch()
    {
        await OnBackToSearch.InvokeAsync();
    }

    private async Task HandleCancel()
    {
        await OnCancel.InvokeAsync();
    }

    private async Task HandleClose()
    {
        await OnClose.InvokeAsync();
    }

    private async Task OnSearchEmailChange(string value)
    {
        await SearchEmailChanged.InvokeAsync(value);
    }

    private string GetRoleDisplayName(UserRole role) => role switch
    {
        UserRole.Owner => "Owner",
        UserRole.Admin => "Admin",
        UserRole.Member => "Member",
        _ => "Member"
    };

    private string GetRoleDescription(UserRole role) => role switch
    {
        UserRole.Admin => "Can manage tasks, lists and invite other members",
        UserRole.Member => "Can create and manage tasks, view board content",
        _ => "Basic access to view and edit tasks"
    };
}