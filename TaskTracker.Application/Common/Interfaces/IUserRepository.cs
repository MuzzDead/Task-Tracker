using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
}

