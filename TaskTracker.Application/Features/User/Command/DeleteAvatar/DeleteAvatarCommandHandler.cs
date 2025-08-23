using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Storage;

namespace TaskTracker.Application.Features.User.Command.DeleteAvatar;

public class DeleteAvatarCommandHandler : IRequestHandler<DeleteAvatarCommand, UserDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;

    public DeleteAvatarCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IBlobService blobService,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _blobService = blobService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(DeleteAvatarCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var user = await uow.Users.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException("User not found");

        if (!user.AvatarId.HasValue)
        {
            throw new BadRequestException("User doesn't have an avatar");
        }

        await _blobService.DeleteAsync(user.AvatarId.Value, cancellationToken);

        user.AvatarId = null;
        await uow.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }
}
