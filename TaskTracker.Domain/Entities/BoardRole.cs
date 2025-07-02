using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class BoardRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid BoardId { get; set; }
    public UserRole Role { get; set; }
}
