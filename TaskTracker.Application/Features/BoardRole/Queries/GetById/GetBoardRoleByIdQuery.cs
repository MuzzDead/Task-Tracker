using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.BoardRole.Queries.GetById;

public class GetBoardRoleByIdQuery : IRequest<BoardRoleDto>
{
    public Guid Id { get; set; }
}
