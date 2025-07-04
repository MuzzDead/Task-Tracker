using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Board.Queries.GetAll;

public class GetAllBoardsQueryHandler : IRequestHandler<GetAllBoardsQuery, IEnumerable<BoardDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    public GetAllBoardsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetAllBoardsQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var boards = await uow.Boards.GetAllAsync();
        if (!request.IncludeArchived)
        {
            boards = boards.Where(b => !b.IsArchived);
        }

        return _mapper.Map<IEnumerable<BoardDto>>(boards);
    }
}
