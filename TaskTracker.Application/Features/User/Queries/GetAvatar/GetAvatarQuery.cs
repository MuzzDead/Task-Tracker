using MediatR;

namespace TaskTracker.Application.Features.User.Queries.GetAvatar;

public class GetAvatarQuery : IRequest<string>
{
    public Guid UserId { get; set; }
}
