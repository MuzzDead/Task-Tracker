using MediatR;

namespace TaskTracker.Application.Features.BoardRole.Command.Delete;

public class DeleteBoardRoleCommand : IRequest
{
    public Guid Id { get; set; }
}
