using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Board.Commands.Delete;

public class DeleteBoardCommand : IRequest
{
    public Guid Id { get; set; }
    public DeleteBoardCommand(Guid id)
    {
        Id = id;
    }
}
