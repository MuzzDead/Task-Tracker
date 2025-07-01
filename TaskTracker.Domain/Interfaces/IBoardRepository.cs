using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Interfaces;

public interface IBoardRepository : IRepository<Board>
{
    Task<IEnumerable<Board>> GetBoardsForUserAsync(Guid userId);
    Task<Board?> GetFullBoardByIdAsync(Guid boardId);
}
