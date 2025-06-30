using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    public virtual ICollection<Board> OwnedBoards { get; private set; } = new List<Board>();
    public virtual ICollection<BoardMember> BoardMemberships { get; private set; } = new List<BoardMember>();
    public virtual ICollection<TaskItem> AssignedTasks { get; private set; } = new List<TaskItem>();
    public virtual ICollection<Comment> Comments { get; private set; } = new List<Comment>();

}
