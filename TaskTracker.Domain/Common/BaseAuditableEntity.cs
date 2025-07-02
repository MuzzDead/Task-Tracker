using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;

    public DateTimeOffset UpdatedAt { get; set; }
    public string? UpdatedBy { get; set;}
}
