namespace TaskTracker.Client.DTOs.Auth;

public class RevokeTokenCommand
{
    public string RefreshToken { get; set; } = string.Empty;
}
