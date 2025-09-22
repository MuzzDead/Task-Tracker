using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Storage;

namespace TaskTracker.Application.Features.User.Queries.GetAvatar;

public class GetAvatarQueryHandler : IRequestHandler<GetAvatarQuery, string>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IBlobService _blobService;

    public GetAvatarQueryHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IBlobService blobService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _blobService = blobService;
    }

    public async Task<string> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var user = await uow.Users.GetByIdAsync(request.UserId);
        if (user?.AvatarId == null) return null;

        return _blobService.GenerateSasToken(user.AvatarId.Value, "avatars");
    }
}
