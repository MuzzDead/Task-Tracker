using MediatR;
using Microsoft.AspNetCore.Http;
using TaskTracker.Application.Common.Interfaces.Services;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.User.Command.RevokeToken;

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;

    public RevokeTokenCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
    }

    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var refreshToken = await uow.RefreshTokens.GetByTokenAsync(request.RefreshToken);
        if (refreshToken == null || !refreshToken.IsActive)
            throw new NotFoundException("Token not found");

        var currentUserId = _currentUserService.GetCurrentUserId();
        if (refreshToken.UserId != currentUserId)
            throw new ForbiddenException("Cannot revoke token for different user");

        var clientIp = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        refreshToken.RevokedAt = DateTimeOffset.UtcNow;
        refreshToken.RevokedByIp = clientIp;

        await uow.SaveChangesAsync();
    }
}
