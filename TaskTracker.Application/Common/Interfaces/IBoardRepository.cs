using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Common.Interfaces;

public interface IBoardRepository : IRepository<Board>
{
    Task CreateBoardAsync(Board board, Guid userId, UserRole userRole);
    Task RemoveUserFromBoardAsync(Guid boardId,Guid userId);
    Task ArchiveAsync(Guid boardId);
}
