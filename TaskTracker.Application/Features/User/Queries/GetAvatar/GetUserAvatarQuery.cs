using MediatR;
using TaskTracker.Application.Storage;

namespace TaskTracker.Application.Features.User.Queries.GetAvatar;

public class GetUserAvatarQuery : IRequest<FileResponse>
{
    public Guid UserId { get; set; }
}
