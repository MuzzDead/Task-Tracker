using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Board.Queries.GetById;

public class GetBoardByIdQueryHandler : IRequestHandler<GetBoardByIdQuery, BoardDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public GetBoardByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<BoardDto> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var board = await uow.Boards.GetByIdAsync(request.Id);

        return board == null ? null : _mapper.Map<BoardDto>(board);
    }
}
