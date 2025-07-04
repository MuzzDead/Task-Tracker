using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Board.Commands.RemoveUser;

public class RemoveUserFromBoardCommand : IRequest
{
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
 
    public RemoveUserFromBoardCommand(Guid boardId, Guid userId)
    {
        BoardId = boardId;
        UserId = userId;
    }
}
