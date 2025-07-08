using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.Auth;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.User.Command.LoginUser;

public class LoginUserComandHandler : IRequestHandler<LoginUserComand, AuthResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenProvider _jwtTokenProvider;
    private readonly IMapper _mapper;
    public LoginUserComandHandler(
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

    public async Task<AuthResponse> Handle(LoginUserComand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var user = await uow.Users.GetByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedException("Invalid credentials");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials");

        var token = _jwtTokenProvider.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            User = _mapper.Map<UserDto>(user)
        };
    }
}
