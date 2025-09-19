using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Member;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Components.Member;

public partial class MemberCard : ComponentBase
{
    private bool dropdownVisible = false;
    private bool roleDrawerVisible = false;
    private bool isUpdatingRole = false;
    private UserRole selectedRole;
    private string? UserAvatar;

    [Parameter] public MemberDto Member { get; set; } = default!;
    [Parameter] public EventCallback<(Guid BoardRoleId, UserRole NewRole)> OnEditRole { get; set; }
    [Parameter] public EventCallback<Guid> OnRemoveMember { get; set; }
    [Parameter] public Guid? CurrentUserId { get; set; }

    [Inject] private IUserService UserService { get; set; } = default!;

    private bool IsCurrentUser => CurrentUserId.HasValue && CurrentUserId.Value == Member.UserId;

    protected override async Task OnInitializedAsync()
    {
        await LoadAvatar();
    }

    private async Task LoadAvatar()
    {
        string? avatarResponse = null;

        try
        {
            avatarResponse = await UserService.GetAvatarUrlAsync(Member.UserId);

            if (!string.IsNullOrEmpty(avatarResponse))
            {
                var jsonDoc = System.Text.Json.JsonDocument.Parse(avatarResponse);
                if (jsonDoc.RootElement.TryGetProperty("avatarUrl", out var avatarUrlElement))
                {
                    avatarResponse = avatarUrlElement.GetString();
                }
            }

            if (!string.IsNullOrEmpty(avatarResponse) && avatarResponse.Contains("sig="))
            {
                var separator = avatarResponse.Contains('?') ? '&' : '?';
                avatarResponse = $"{avatarResponse}{separator}t={DateTime.UtcNow.Ticks}";
            }

            UserAvatar = string.IsNullOrEmpty(avatarResponse)
                ? GeneratePlaceholderAvatar()
                : avatarResponse;
        }
        catch
        {
            UserAvatar = GeneratePlaceholderAvatar();
        }
    }

    private string GeneratePlaceholderAvatar()
    {
        var seedName = Member.Username ?? "user";
        return $"https://api.dicebear.com/7.x/identicon/svg?seed={seedName}&t={DateTime.UtcNow.Ticks}";
    }

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