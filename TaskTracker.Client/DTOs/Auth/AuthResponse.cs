using TaskTracker.Client.DTOs.User;

namespace TaskTracker.Client.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}