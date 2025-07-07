using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Card.Queries.GetByColumnId;

public class GetCardsByColumnIdQueryHandler : IRequestHandler<GetCardsByColumnIdQuery, IEnumerable<CardDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public GetCardsByColumnIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CardDto>> Handle(GetCardsByColumnIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var cards = await uow.Cards.GetByColumnIdAsync(request.ColumnId);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }
}
