using AntDesign;
using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.User;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.Profile;

public partial class Profile : ComponentBase
{
    [Inject] NavigationManager Navigation { get; set; } = default!;
    [Inject] IAuthStateService AuthStateService { get; set; } = default!;
    [Inject] IAuthService AuthService { get; set; } = default!;
    [Inject] IUserService UserService { get; set; } = default!;
    [Inject] MessageService MessageService { get; set; } = default!;
    [Inject] IConfiguration Config { get; set; } = default!;

    private UserDto? User;
    private bool IsLoading = true;
    private bool showLogout = false;
    private bool isAuthenticated = false;
    private Guid currentUserId = Guid.Empty;
    private string? currentAvatarUrl;

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

            await LoadAvatar();
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

    private async Task LoadAvatar()
    {
        try
        {
            if (User == null)
            {
                currentAvatarUrl = GeneratePlaceholderAvatar();
                return;
            }

            try
            {
                var avatarResponse = await UserService.GetAvatarUrlAsync(User.Id);

                if (!string.IsNullOrEmpty(avatarResponse))
                {
                    try
                    {
                        var jsonDoc = System.Text.Json.JsonDocument.Parse(avatarResponse);
                        if (jsonDoc.RootElement.TryGetProperty("avatarUrl", out var avatarUrlElement))
                        {
                            currentAvatarUrl = avatarUrlElement.GetString();
                        }
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        currentAvatarUrl = avatarResponse;
                    }
                }

                if (!string.IsNullOrEmpty(currentAvatarUrl) && currentAvatarUrl.Contains("sig="))
                {
                    var separator = currentAvatarUrl.Contains('?') ? '&' : '?';
                    currentAvatarUrl = $"{currentAvatarUrl}{separator}t={DateTime.UtcNow.Ticks}";
                }

                if (string.IsNullOrEmpty(currentAvatarUrl))
                {
                    currentAvatarUrl = GeneratePlaceholderAvatar();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting avatar from API: {ex.Message}");
                currentAvatarUrl = string.IsNullOrEmpty(User.AvatarUrl)
                    ? GeneratePlaceholderAvatar()
                    : User.AvatarUrl;
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading avatar: {ex.Message}");
            currentAvatarUrl = GeneratePlaceholderAvatar();
            StateHasChanged();
        }
    }

    private string GeneratePlaceholderAvatar()
    {
        return $"https://api.dicebear.com/7.x/identicon/svg?seed={User?.Username ?? "user"}";
    }

    private void OnEditInfo() => Navigation.NavigateTo("/editprofile");
    private void OnChangePassword() => Navigation.NavigateTo("/changepassword");
    private void GoToLogin() => Navigation.NavigateTo("/login");
}