using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class BoardMember : BaseEntity
{
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public Role Role  { get; set; } = Role.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}