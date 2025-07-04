using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Board.Commands.Archive;

public class ArchiveBoardCommand : IRequest
{
    public Guid Id { get; set; }

    public ArchiveBoardCommand(Guid id)
    {
        Id = id;
    }
}
