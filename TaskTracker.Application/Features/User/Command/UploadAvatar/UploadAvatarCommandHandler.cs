using MediatR;
using Microsoft.Extensions.Logging;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Storage;

namespace TaskTracker.Application.Features.User.Command.UploadAvatar;

public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IBlobService _blobService;
    public UploadAvatarCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IBlobService blobService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _blobService = blobService;
    }

    public async Task<Guid> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var user = await uow.Users.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException("User not found");

        if (user.AvatarId.HasValue)
        {
            await _blobService.DeleteAsync(user.AvatarId.Value, "avatars");
        }

        var newAvatarId = await _blobService.UploadAsync(
            request.FileStream, request.ContentType, "avatars");

        user.AvatarId = newAvatarId;
        await uow.SaveChangesAsync(cancellationToken);

        return newAvatarId;
    }
}