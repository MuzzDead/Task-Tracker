using MediatR;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Board.Commands.Create;

public class CreateBoardCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public UserRole UserRole { get; set; } = UserRole.Admin;
}
