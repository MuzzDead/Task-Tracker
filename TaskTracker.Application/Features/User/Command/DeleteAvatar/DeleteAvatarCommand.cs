using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.User.Command.DeleteAvatar;

public class DeleteAvatarCommand : IRequest<UserDto>
{
    public Guid UserId { get; set; }
}