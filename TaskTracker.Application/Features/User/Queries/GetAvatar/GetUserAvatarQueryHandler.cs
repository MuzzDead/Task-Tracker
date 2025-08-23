using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Storage;

namespace TaskTracker.Application.Features.User.Queries.GetAvatar;

public class GetUserAvatarQueryHandler : IRequestHandler<GetUserAvatarQuery, FileResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IBlobService _blobService;

    public GetUserAvatarQueryHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IBlobService blobService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _blobService = blobService;
    }

    public async Task<FileResponse> Handle(GetUserAvatarQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var user = await uow.Users.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException("User not found");

        if (!user.AvatarId.HasValue)
        {
            throw new NotFoundException("User avatar not found");
        }

        return await _blobService.DownloadAsync(user.AvatarId.Value, cancellationToken);
    }
}
