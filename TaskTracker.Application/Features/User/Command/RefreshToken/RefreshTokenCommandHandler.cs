using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using TaskTracker.Application.Common.Interfaces.Auth;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;
using TaskTracker.Domain.Options;

namespace TaskTracker.Application.Features.User.Command.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IJwtTokenProvider _jwtTokenProvider;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtOptions _jwtOptions;
    public RefreshTokenCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IJwtTokenProvider jwtTokenProvider,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> jwtOptions)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _jwtTokenProvider = jwtTokenProvider;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        ClaimsPrincipal principal;
        try
        {
            principal = _jwtTokenProvider.GetPrincipalFromExpiredToken(request.AccessToken);
        }
        catch
        {
            throw new UnauthorizedException("Invalid access token");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedException("Invalid access token claims");

        var refreshTokenEntity = await uow.RefreshTokens.GetByTokenAsync(request.RefreshToken);
        if (refreshTokenEntity == null || !refreshTokenEntity.IsActive || refreshTokenEntity.UserId != userId)
            throw new UnauthorizedException("Invalid refresh token");

        var user = await uow.Users.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedException("User not found");

        var clientIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        refreshTokenEntity.RevokedAt = DateTimeOffset.UtcNow;
        refreshTokenEntity.RevokedByIp = clientIp;

        var newAccessToken = _jwtTokenProvider.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenProvider.GenerateRefreshToken();

        refreshTokenEntity.ReplacedByToken = newRefreshToken;

        var newRefreshTokenEntity = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            CreatedByIp = clientIp
        };

        await uow.RefreshTokens.AddAsync(newRefreshTokenEntity);
        await uow.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            User = _mapper.Map<UserDto>(user),
            ExpiresAt = DateTime.UtcNow.AddHours(_jwtOptions.ExpirationHours)
        };

    }
}
