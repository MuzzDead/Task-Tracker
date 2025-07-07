using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Persistence.Repositories.Base;

namespace TaskTracker.Persistence.Repositories;

public class UserRepository : BaseRepository<User, Guid>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _dbSet.AnyAsync(u => u.Email == email);
    }
}
