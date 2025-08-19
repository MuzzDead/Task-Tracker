using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces.Auth;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    Task AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task RevokeAllUserTokensAsync(Guid userId, string revokedByIp);
    Task RemoveExpiredTokensAsync();
}
