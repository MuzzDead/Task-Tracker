using System.Security.Claims;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces.Auth;

public interface IJwtTokenProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
