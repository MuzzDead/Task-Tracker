using MediatR;

namespace TaskTracker.Application.Features.Column.Commands.Delete;

public class DeleteColumnCommand : IRequest
{
    public Guid Id { get; set; }
}
