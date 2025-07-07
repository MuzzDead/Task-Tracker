using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Column.Queries.GetById;

public class GetColumnByIdQuery : IRequest<ColumnDto>
{
    public Guid Id { get; set; }
}
