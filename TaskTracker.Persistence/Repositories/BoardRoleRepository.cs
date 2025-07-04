using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Persistence.Repositories.Base;

namespace TaskTracker.Persistence.Repositories;

public class BoardRoleRepository : BaseRepository<BoardRole, Guid>, IBoardRoleRepository
{
    public BoardRoleRepository(ApplicationDbContext context) : base(context) { }
}
