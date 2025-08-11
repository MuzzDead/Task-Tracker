using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Pagination;

namespace TaskTracker.Application.Features.Board.Queries.GetByUserId;

public class GetBoardsByUserIdQueryHandler : IRequestHandler<GetBoardsByUserIdQuery, PagedResult<BoardDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    public GetBoardsByUserIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<PagedResult<BoardDto>> Handle(GetBoardsByUserIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var result = await uow.Boards.GetByUserIdAsync(
                    request.UserId,
                    request.PagedRequest);

        var boardDtos = _mapper.Map<IEnumerable<BoardDto>>(result.Items);

        return new PagedResult<BoardDto>(
            items: boardDtos,
            totalCount: result.TotalCount,
            page: result.Page,
            pageSize: result.PageSize);
    }
}
