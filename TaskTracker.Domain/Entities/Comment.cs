using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class Comment : BaseAuditableEntity
{
    public string Text { get; set; } = string.Empty;
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
}