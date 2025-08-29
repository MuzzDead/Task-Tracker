using MediatR;

namespace TaskTracker.Application.Features.User.Command.UploadAvatar;

public class UploadAvatarCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Stream FileStream { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}