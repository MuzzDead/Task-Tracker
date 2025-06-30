using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class BoardMember : BaseEntity
{
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = "Member";
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    public virtual Board Board { get; private set; } = null!;
    public virtual User User { get; private set; } = null!;
}