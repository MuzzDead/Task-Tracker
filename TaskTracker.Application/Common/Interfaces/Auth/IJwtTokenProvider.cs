using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces.Auth;

public interface IJwtTokenProvider
{
    string GenerateToken(User user);
}
