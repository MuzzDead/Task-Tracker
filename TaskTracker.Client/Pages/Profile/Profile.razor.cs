using AntDesign;
using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.User;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.Profile;

public partial class Profile
{
    [Inject] NavigationManager Navigation { get; set; } = default!;
    [Inject] IAuthStateService AuthStateService { get; set; } = default!;
    [Inject] IAuthService AuthService { get; set; } = default!;
    [Inject] MessageService MessageService { get; set; } = default!;
    [Inject] IConfiguration Config { get; set; } = default!;

    private UserDto? User;
    private bool IsLoading = true;
    private bool showLogout = false;
    private bool isAuthenticated = false;
    private Guid currentUserId = Guid.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserProfile();
    }

    private async Task LoadUserProfile()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            await AuthStateService.InitializeAsync();

            if (!AuthStateService.IsAuthenticated || AuthStateService.CurrentUser == null)
            {
                return;
            }

            isAuthenticated = true;
            currentUserId = AuthStateService.CurrentUser.Id;
            User = AuthStateService.CurrentUser;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading profile: {ex.Message}");
            await AuthStateService.ClearAuthDataAsync();
            User = null;
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
    private string? currentAvatarUrl => User?.AvatarId != null
        ? $"{Config["ApiBaseUrl"]}/user/{User.Id}/avatar"
        : $"https://api.dicebear.com/7.x/identicon/svg?seed={User?.Username ?? "user"}";

    private void OnEditInfo() => Navigation.NavigateTo("/editprofile");

    private void OnChangePassword() => Navigation.NavigateTo("/changepassword");

    private void GoToLogin() => Navigation.NavigateTo("/login");
}
