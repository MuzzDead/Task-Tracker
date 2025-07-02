using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class Column : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public int ColumnIndex { get; set; }
    public Guid BoardId { get; set; }
}