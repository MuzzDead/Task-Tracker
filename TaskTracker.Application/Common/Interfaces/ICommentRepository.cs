using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
}
