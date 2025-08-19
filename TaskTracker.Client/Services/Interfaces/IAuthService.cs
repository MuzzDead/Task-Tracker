using Refit;
using TaskTracker.Client.DTOs.Auth;
using TaskTracker.Client.DTOs.User;

namespace TaskTracker.Client.Services.Interfaces;

public interface IAuthService
{
    [Post("/auth/register")]
    Task<AuthResponse> RegisterAsync([Body] RegisterUserDto model);

    [Post("/auth/login")]
    Task<AuthResponse> LoginAsync([Body] LoginUserDto model);

    [Get("/auth/me")]
    Task<UserDto> GetCurrentUserAsync();

    [Post("/auth/refresh-token")]
    Task<AuthResponse> RefreshTokenAsync([Body] RefreshTokenRequest request);

    [Post("/auth/revoke-token")]
    Task RevokeTokenAsync(RevokeTokenCommand command);
}
