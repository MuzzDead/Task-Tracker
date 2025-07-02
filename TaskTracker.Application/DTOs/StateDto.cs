using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Enums;


namespace TaskTracker.Application.DTOs;

public class StateDto
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public Status Status { get; set; }
    public Priority Priority { get; set; }
    public Guid CardId { get; set; }
}
