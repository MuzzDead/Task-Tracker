using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Storage;


namespace TaskTracker.Application.Features.User.Queries.GetById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;
    public GetUserByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IBlobService blobService, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _blobService = blobService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var user = await uow.Users.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"User with ID {request.Id} was not found.");

        var userDto = _mapper.Map<UserDto>(user);

        if (user.AvatarId.HasValue)
        {
            userDto.AvatarUrl = _blobService.GenerateSasToken(user.AvatarId.Value, "avatars");
        }

        return userDto;
    }
}
