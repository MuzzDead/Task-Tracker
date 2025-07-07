using MediatR;

namespace TaskTracker.Application.Features.User.Command.Update;

public class UpdateUserCommand : IRequest
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
