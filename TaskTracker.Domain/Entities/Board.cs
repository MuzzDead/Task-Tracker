using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class Board : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BackgroundColor { get; set; }
    public BoardVisibility Visibility { get; set; } = BoardVisibility.Private;
    public bool IsArchived { get; set; } = false;
    public Guid OwnerId { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }

    public virtual User Owner { get; set; } = null!;
    public virtual ICollection<BoardMember> Members { get; set; } = new List<BoardMember>();
    public virtual ICollection<TaskState> States { get; set; } = new List<TaskState>();
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
