using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.Services;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.User.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    public GetCurrentUserQueryHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var user = await uow.Users.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedException("User not found");

        return _mapper.Map<UserDto>(user);
    }
}
