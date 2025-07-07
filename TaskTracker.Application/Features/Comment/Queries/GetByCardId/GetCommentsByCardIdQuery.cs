using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Comment.Queries.GetByCardId;

public class GetCommentsByCardIdQuery : IRequest<IEnumerable<CommentDto>>
{
    public Guid CardId { get; set; }
}
