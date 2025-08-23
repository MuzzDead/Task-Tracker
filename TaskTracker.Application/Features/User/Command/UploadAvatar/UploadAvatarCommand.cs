using MediatR;
using Microsoft.AspNetCore.Http;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.User.Command.UploadAvatar;

public class UploadAvatarCommand : IRequest<UserDto>
{
    public Guid UserId { get; set; }
    public IFormFile Avatar { get; set; } = null!;
}