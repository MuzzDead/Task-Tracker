using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class Card : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public Guid ColumnId {  get; set; }
    public int RowIndex {  get; set; }
}