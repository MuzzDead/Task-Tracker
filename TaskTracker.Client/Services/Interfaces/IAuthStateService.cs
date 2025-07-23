using TaskTracker.Client.DTOs.Auth;
using TaskTracker.Client.DTOs.User;

namespace TaskTracker.Client.Services.Interfaces;

public interface IAuthStateService
{
    bool IsAuthenticated { get; }
    string? Token { get; }
    UserDto? CurrentUser { get; }
    Task SetAuthDataAsync(AuthResponse authResponse);
    Task ClearAuthDataAsync();
    Task InitializeAsync();
    event Action<bool> AuthStateChanged;
}
