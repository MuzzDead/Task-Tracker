using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.BoardRole.Queries.GetAll;

public class GetAllBoardRoleQuery : IRequest<IEnumerable<BoardRoleDto>>
{
    public Guid Id { get; set; }
}
