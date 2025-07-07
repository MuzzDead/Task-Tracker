using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Comment.Queries.GetById;

public class GetCommentByIdQuery : IRequest<CommentDto?>
{
    public Guid Id { get; set; }
}
