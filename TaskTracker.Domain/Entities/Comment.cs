using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class Comment : BaseEntity, IAuditableEntity
{
    public string Content { get; private set; } = string.Empty;
    public bool IsEdited { get; private set; } = false;
    public DateTime? EditedAt { get; private set; }

    public Guid TaskId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }

    public virtual TaskItem Task { get; private set; } = null!;
    public virtual User Author { get; private set; } = null!;
}
