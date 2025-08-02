using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Member;

namespace TaskTracker.Client.Components.Member;

public partial class MemberCard : ComponentBase
{
    private bool dropdownVisible = false;
    private bool roleDrawerVisible = false;
    private bool isUpdatingRole = false;
    private UserRole selectedRole;

    [Parameter] public MemberDto Member { get; set; } = default!;
    [Parameter] public EventCallback<(Guid BoardRoleId, UserRole NewRole)> OnEditRole { get; set; }
    [Parameter] public EventCallback<Guid> OnRemoveMember { get; set; }

    private void ShowRoleDrawer()
    {
        selectedRole = Member.UserRole;
        dropdownVisible = false;
        roleDrawerVisible = true;
        StateHasChanged();
    }

    private void CloseRoleDrawer()
    {
        roleDrawerVisible = false;
        selectedRole = Member.UserRole;
        isUpdatingRole = false;
        StateHasChanged();
    }

    private async Task HandleRoleUpdate()
    {
        if (selectedRole == Member.UserRole)
        {
            CloseRoleDrawer();
            return;
        }

        isUpdatingRole = true;
        var originalRole = Member.UserRole;
        StateHasChanged();

        try
        {
            await OnEditRole.InvokeAsync((Member.BoardRoleId, selectedRole));
            CloseRoleDrawer();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating role: {ex.Message}");
            selectedRole = originalRole;
            Member.UserRole = originalRole;
            isUpdatingRole = false;
            StateHasChanged();
        }
    }

    private async Task HandleDelete()
    {
        dropdownVisible = false;
        StateHasChanged();

        try
        {
            await OnRemoveMember.InvokeAsync(Member.BoardRoleId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing member: {ex.Message}");
        }
    }

    private string GetRoleColor(UserRole role) => role switch
    {
        UserRole.Owner => "gold",
        UserRole.Admin => "blue",
        UserRole.Member => "green",
        _ => "default"
    };

    private string GetRoleDisplayName(UserRole role) => role switch
    {
        UserRole.Owner => "Owner",
        UserRole.Admin => "Admin",
        UserRole.Member => "Member",
        _ => "Member"
    };
}