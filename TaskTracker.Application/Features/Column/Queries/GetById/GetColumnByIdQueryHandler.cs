using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Column.Queries.GetById;

public class GetColumnByIdQueryHandler : IRequestHandler<GetColumnByIdQuery, ColumnDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    public GetColumnByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<ColumnDto> Handle(GetColumnByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var column = await uow.Columns.GetByIdAsync(request.Id);

        return column == null ? null : _mapper.Map<ColumnDto>(column);
    }
}
