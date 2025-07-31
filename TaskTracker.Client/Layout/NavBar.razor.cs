using Microsoft.AspNetCore.Components;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Layout;

public partial class NavBar : ComponentBase, IDisposable
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IAuthStateService AuthStateService { get; set; } = default!;

    private bool showLogout = false;
    private bool isAuthenticated = false;
    private string userDisplayName = string.Empty;

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
        UpdateAuthState();
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
        InvokeAsync(() =>
        {
            UpdateAuthState();
            StateHasChanged();
        });
    }

    private void UpdateAuthState()
    {
        isAuthenticated = AuthStateService.IsAuthenticated;
        userDisplayName = AuthStateService.CurrentUser?.Username ?? string.Empty;
    }

    public void Dispose()
    {
        AuthStateService.AuthStateChanged -= OnAuthStateChanged;
    }
}
