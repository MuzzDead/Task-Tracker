using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.DTOs;

public class BoardRoleDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BoardId { get; set; }
    public UserRole UserRole { get; set; }
}
