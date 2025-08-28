using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.User.Command.DeleteAvatar;

public class DeleteAvatarCommand : IRequest
{
    public Guid UserId { get; set; }
}