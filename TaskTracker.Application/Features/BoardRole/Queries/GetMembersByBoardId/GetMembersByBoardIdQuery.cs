using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.BoardRole.Queries.GetMembersByBoardId;

public class GetMembersByBoardIdQuery : IRequest<IEnumerable<MemberDto>>
{
    public Guid BoardId { get; set; }
}