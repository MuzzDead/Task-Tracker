using Microsoft.AspNetCore.Components;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Layout;

public partial class NavBar : ComponentBase, IDisposable
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IAuthStateService AuthStateService { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;

    private bool showLogout = false;
    private bool isAuthenticated = false;
    private string userDisplayName = string.Empty;
    private string? userAvatarUrl;

    private void ToggleLogout()
    {
        if (isAuthenticated)
        {
            showLogout = !showLogout;
        }
    }

    private async Task LogoutAsync()
    {
        await AuthStateService.ClearAuthDataAsync();
        isAuthenticated = false;
        userDisplayName = string.Empty;
        showLogout = false;
        Navigation.NavigateTo("/");
    }

    protected override async Task OnInitializedAsync()
    {
        AuthStateService.AuthStateChanged += OnAuthStateChanged;

        await AuthStateService.InitializeAsync();
        await UpdateAuthState();
    }

    private void NavigateTo(string path)
    {
        Navigation.NavigateTo(path);
    }

    private bool IsActive(string path)
    {
        var currentPath = Navigation.Uri.Replace(Navigation.BaseUri, "/");
        return currentPath.StartsWith(path, StringComparison.OrdinalIgnoreCase);
    }

    private void OnAuthStateChanged(bool isAuthenticatedNow)
    {
        InvokeAsync(async () =>
        {
            await UpdateAuthState();
            StateHasChanged();
        });
    }

    private async Task UpdateAuthState()
    {
        isAuthenticated = AuthStateService.IsAuthenticated;
        userDisplayName = AuthStateService.CurrentUser?.Username ?? string.Empty;

        if (isAuthenticated && AuthStateService.CurrentUser is not null)
        {
            await LoadAvatarAsync();
        }
        else
        {
            userAvatarUrl = null;
        }
    }

    private async Task LoadAvatarAsync()
    {
        var userId = AuthStateService.CurrentUser?.Id;
        if (userId is null)
        {
            userAvatarUrl = GeneratePlaceholderAvatar();
            return;
        }

        string? avatarResponse = null;
        try
        {
            avatarResponse = await UserService.GetAvatarUrlAsync(userId.Value);

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

            userAvatarUrl = string.IsNullOrEmpty(avatarResponse)
                ? GeneratePlaceholderAvatar()
                : avatarResponse;
        }
        catch
        {
            userAvatarUrl = GeneratePlaceholderAvatar();
        }
    }

    private string GeneratePlaceholderAvatar()
    {
        var seedName = userDisplayName ?? "guest";
        return $"https://api.dicebear.com/7.x/identicon/svg?seed={seedName}&t={DateTime.UtcNow.Ticks}";
    }

    public void Dispose()
    {
        AuthStateService.AuthStateChanged -= OnAuthStateChanged;
    }
}
