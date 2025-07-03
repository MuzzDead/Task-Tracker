using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Common.Interfaces;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User> GetByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<UserRole> GetUserRoleAsync(Guid userId, Guid boarId);
}

