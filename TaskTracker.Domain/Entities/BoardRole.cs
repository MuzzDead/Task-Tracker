using TaskTracker.Domain.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class BoardRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid BoardId { get; set; }
    public UserRole Role { get; set; }
}
