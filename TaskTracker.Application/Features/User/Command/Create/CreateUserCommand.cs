using MediatR;

namespace TaskTracker.Application.Features.User.Command.Create;

public class CreateUserCommand : IRequest<Guid>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
