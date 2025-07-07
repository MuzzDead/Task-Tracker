using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Column.Queries.GetByBoardId;

public class GetColumnsByBoardIdQueryHandler : IRequestHandler<GetColumnsByBoardIdQuery, IEnumerable<ColumnDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    public GetColumnsByBoardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ColumnDto>> Handle(GetColumnsByBoardIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var cards = await uow.Columns.GetByBoardIdAsync(request.BoardId);

        return _mapper.Map<IEnumerable<ColumnDto>>(cards);
    }
}
