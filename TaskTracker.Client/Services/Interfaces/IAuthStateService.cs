using TaskTracker.Client.DTOs.Auth;
using TaskTracker.Client.DTOs.User;

namespace TaskTracker.Client.Services.Interfaces;

public interface IAuthStateService
{
    bool IsAuthenticated { get; }
    string? AccessToken { get; }
    string? RefreshToken { get; }
    UserDto? CurrentUser { get; }
    DateTime? TokenExpiresAt { get; }

    Task SetAuthDataAsync(AuthResponse authResponse);
    Task RefreshTokenAsync();
    Task ClearAuthDataAsync();
    Task InitializeAsync();

    event Action<bool> AuthStateChanged;
}
