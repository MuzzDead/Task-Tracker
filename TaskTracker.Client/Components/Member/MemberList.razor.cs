using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Member;
using TaskTracker.Client.DTOs.User;
using static TaskTracker.Client.Pages.BoardDetails.BoardDetails;

namespace TaskTracker.Client.Components.Member;

public partial class MemberList : ComponentBase
{
    [Parameter] public Guid BoardId { get; set; }
    [Parameter] public Guid? CurrentUserId { get; set; }

    [Parameter] public List<MemberDto> Members { get; set; } = new();
    [Parameter] public bool MembersDrawerVisible { get; set; }
    [Parameter] public bool InviteModalVisible { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public string ErrorMessage { get; set; } = string.Empty;
    
    [Parameter] public InviteStep CurrentInviteStep { get; set; }
    [Parameter] public string SearchEmail { get; set; } = "";
    [Parameter] public EventCallback<string> SearchEmailChanged { get; set; }
    [Parameter] public string SearchError { get; set; } = "";
    [Parameter] public UserDto? FoundUser { get; set; }
    [Parameter] public UserRole SelectedRole { get; set; } = UserRole.Member;
    [Parameter] public EventCallback<UserRole> SelectedRoleChanged { get; set; }
    [Parameter] public bool IsSearching { get; set; }
    [Parameter] public bool IsSendingInvite { get; set; }
    
    [Parameter] public EventCallback OnMembersDrawerClose { get; set; }
    [Parameter] public EventCallback OnOpenInviteModal { get; set; }
    [Parameter] public EventCallback OnCloseInviteModal { get; set; }
    [Parameter] public EventCallback<(Guid BoardRoleId, UserRole NewRole)> OnEditRole { get; set; }
    [Parameter] public EventCallback<Guid> OnRemoveMember { get; set; }
    
    [Parameter] public EventCallback OnUserSearch { get; set; }
    [Parameter] public EventCallback OnSendInvite { get; set; }
    [Parameter] public EventCallback OnBackToSearch { get; set; }

    private async Task HandleMembersDrawerClose()
    {
        await OnMembersDrawerClose.InvokeAsync();
    }

    private async Task OpenInviteModal()
    {
        await OnOpenInviteModal.InvokeAsync();
    }

    private async Task HandleEditRole((Guid BoardRoleId, UserRole NewRole) roleUpdate)
    {
        await OnEditRole.InvokeAsync(roleUpdate);
    }

    private async Task HandleRemoveMember(Guid boardRoleId)
    {
        await OnRemoveMember.InvokeAsync(boardRoleId);
    }

    private async Task HandleUserSearch()
    {
        await OnUserSearch.InvokeAsync();
    }

    private async Task HandleSendInvite()
    {
        await OnSendInvite.InvokeAsync();
    }

    private async Task HandleBackToSearch()
    {
        await OnBackToSearch.InvokeAsync();
    }

    private async Task HandleInviteCancel()
    {
        await OnCloseInviteModal.InvokeAsync();
    }

    private async Task HandleInviteClose()
    {
        await OnCloseInviteModal.InvokeAsync();
    }
}