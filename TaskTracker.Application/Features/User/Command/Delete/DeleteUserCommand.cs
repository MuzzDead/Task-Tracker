using MediatR;

namespace TaskTracker.Application.Features.User.Command.Delete;

public class DeleteUserCommand : IRequest
{
    public Guid Id { get; set; }
}
