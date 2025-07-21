using Microsoft.AspNetCore.Components;

namespace TaskTracker.Client.Layout;

public partial class NavBar : ComponentBase
{
    private bool isAuthenticated = true;
    private string userDisplayName = "Test User";

    private void NavigateTo(string path)
    {
        Navigation.NavigateTo(path);
    }

    private bool IsActive(string path)
    {
        var currentPath = Navigation.Uri.Replace(Navigation.BaseUri, "/");
        return currentPath.StartsWith(path, StringComparison.OrdinalIgnoreCase);
    }
}
