using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.Board.Commands.Create;

public class CreateBoardCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public UserRole UserRole { get; set; } = UserRole.Admin;
}
