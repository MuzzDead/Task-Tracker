using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.State.Queries.GetByCardId;

public class GetStateByCardIdQueryHandler : IRequestHandler<GetStateByCardIdQuery, StateDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    public GetStateByCardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<StateDto> Handle(GetStateByCardIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var state = await uow.States.GetByCardIdAsync(request.CardId);
        if (state == null)
        {
            throw new NotFoundException($"State with CardID {request.CardId} was not found.");
        }

        return _mapper.Map<StateDto>(state);
    }
}
