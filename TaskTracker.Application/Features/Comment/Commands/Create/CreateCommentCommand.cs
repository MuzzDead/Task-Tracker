using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Comment.Commands.Create;

public class CreateCommentCommand : IRequest<Guid>
{
    public string Text { get; set; } = string.Empty;
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public ICollection<AttachmentUpload> Attachments { get; set; } = new List<AttachmentUpload>();
}
