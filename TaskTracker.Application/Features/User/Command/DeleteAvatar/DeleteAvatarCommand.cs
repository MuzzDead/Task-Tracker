using MediatR;

namespace TaskTracker.Application.Features.User.Command.DeleteAvatar;

public class DeleteAvatarCommand : IRequest
{
    public Guid UserId { get; set; }
}