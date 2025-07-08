using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces.Auth;

public interface IJwtTokenProvider
{
    Task<string> GenerateTokenAsync(User user);
}
