using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Card.Queries.GetById;

public class GetCardByIdQuery : IRequest<CardDto>
{
    public Guid Id { get; set; }
    public GetCardByIdQuery(Guid id)
    {
        Id = id;
    }
}
