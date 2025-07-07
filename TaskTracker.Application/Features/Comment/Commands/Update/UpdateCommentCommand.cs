using MediatR;

namespace TaskTracker.Application.Features.Comment.Commands.Update;

public class UpdateCommentCommand : IRequest
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public Guid UpdatedBy { get; set; }
}
