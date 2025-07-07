using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Card.Commands.Create;

public class CreateCardCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;
    public Guid ColumnId { get; set; }
    public int RowIndex { get; set; }
    public Guid CreatedBy { get; set; }
}
