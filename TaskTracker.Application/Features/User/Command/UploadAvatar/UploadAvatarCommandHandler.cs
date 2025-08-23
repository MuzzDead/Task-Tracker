using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Storage;

namespace TaskTracker.Application.Features.User.Command.UploadAvatar;

public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, UserDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;

    public UploadAvatarCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IBlobService blobService,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _blobService = blobService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        ValidateAvatarFile.ValidateFile(request.Avatar);

        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var user = await uow.Users.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException("User not found");

        if (user.AvatarId.HasValue)
        {
            await _blobService.DeleteAsync(user.AvatarId.Value, cancellationToken);
        }

        using var stream = request.Avatar.OpenReadStream();
        var newAvatarId = await _blobService.UploadAsync(
            stream,
            request.Avatar.ContentType,
            cancellationToken);

        user.AvatarId = newAvatarId;
        await uow.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }
}
