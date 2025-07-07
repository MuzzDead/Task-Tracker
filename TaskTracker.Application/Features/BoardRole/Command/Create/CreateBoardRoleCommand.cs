using MediatR;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.BoardRole.Command.Create;

public class CreateBoardRoleCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid BoardId { get; set; }
    public UserRole Role { get; set; }
}
