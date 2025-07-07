using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Card.Commands.Delete;

public class DeleteCardCommand : IRequest
{
    public Guid Id { get; set; }

    public DeleteCardCommand(Guid id)
    {
        Id = id;
    }
}
