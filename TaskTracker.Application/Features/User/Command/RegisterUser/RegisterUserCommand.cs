using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.User.Command.RegisterUser;

public class RegisterUserCommand : IRequest<AuthResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
