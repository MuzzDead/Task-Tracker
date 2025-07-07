using MediatR;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.BoardRole.Command.Update;

public class UpdateBoardRoleQuery : IRequest
{
    public Guid Id { get; set; }
    public UserRole Role { get; set; }
}
