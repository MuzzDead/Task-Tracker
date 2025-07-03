using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Domain.Common;

public abstract class ArchivableEntity : BaseAuditableEntity
{
    public bool IsArchived { get; set; }
    public DateTimeOffset ArchivedAt { get; set; }
}
