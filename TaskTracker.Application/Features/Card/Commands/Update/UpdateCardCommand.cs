using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Card.Commands.Update;

public class UpdateCardCommand : IRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid ColumnId { get; set; }
    public int RowIndex { get; set; }
    public Guid UpdatedBy { get; set; }
}
