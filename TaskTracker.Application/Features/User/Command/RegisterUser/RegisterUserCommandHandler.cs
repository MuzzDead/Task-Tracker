using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.Auth;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.User.Command.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenProvider _jwtTokenProvider;
    private readonly IMapper _mapper;
    public RegisterUserCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IPasswordHasher passwordHasher,
        IJwtTokenProvider jwtTokenProvider,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _passwordHasher = passwordHasher;
        _jwtTokenProvider = jwtTokenProvider;
        _mapper = mapper;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var isUniqueEmail = await uow.Users.IsEmailUniqueAsync(request.Email);
        if (!isUniqueEmail)
        {
            throw new ConflictException("Email is already in use");
        }

        var user = new Domain.Entities.User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.Generate(request.Password)
        };

        await uow.Users.AddAsync(user);
        await uow.SaveChangesAsync();

        var token = _jwtTokenProvider.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserDto>(user)
        };
    }
}
