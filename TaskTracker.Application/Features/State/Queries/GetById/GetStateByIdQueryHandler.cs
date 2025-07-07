using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.State.Queries.GetById;

public class GetStateByIdQueryHandler : IRequestHandler<GetStateByIdQuery, StateDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    public GetStateByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<StateDto> Handle(GetStateByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var state = await uow.States.GetByIdAsync(request.Id);
        if (state == null)
        {
            throw new NotFoundException($"State with ID {request.Id} was not found.");
        }

        return _mapper.Map<StateDto>(state);
    }
}
