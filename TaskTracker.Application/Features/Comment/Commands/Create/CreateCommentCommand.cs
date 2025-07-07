using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Application.Features.Comment.Commands.Create;

public class CreateCommentCommand : IRequest<Guid>
{
    public string Text { get; set; } = string.Empty;
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
}
