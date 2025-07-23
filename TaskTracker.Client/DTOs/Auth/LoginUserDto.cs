using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Client.DTOs.Auth;

public class LoginUserDto
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
