using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TaskTracker.Application.Common.Interfaces.Auth;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;
using TaskTracker.Domain.Options;

namespace TaskTracker.Application.Features.User.Command.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenProvider _jwtTokenProvider;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtOptions _jwtOptions;
    public RegisterUserCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IPasswordHasher passwordHasher,
        IJwtTokenProvider jwtTokenProvider,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> jwtOptions)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _passwordHasher = passwordHasher;
        _jwtTokenProvider = jwtTokenProvider;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions.Value;
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

        var accessToken = _jwtTokenProvider.GenerateAccessToken(user);
        var refreshToken = _jwtTokenProvider.GenerateRefreshToken();

        var clientIp = GetClientIp();
        var refreshTokenEntity = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            CreatedByIp = clientIp
        };

        await uow.RefreshTokens.AddAsync(refreshTokenEntity);
        await uow.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = _mapper.Map<UserDto>(user),
            ExpiresAt = DateTime.UtcNow.AddHours(_jwtOptions.ExpirationHours)
        };
    }

    private string GetClientIp()
    {
        return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}
