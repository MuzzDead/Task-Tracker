using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid AuthorId { get; set; }
    public string Text { get; set; } = string.Empty;
    public Guid TaskId { get; set; }
    public bool? IsEdited { get; set; } = false;
    public DateTime? EdietedAt { get; set; }
}