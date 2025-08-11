using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Pagination;

namespace TaskTracker.Application.Features.Board.Queries.Search;

public class SearchBoardsQueryHandler : IRequestHandler<SearchBoardsQuery, PagedResult<BoardDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public SearchBoardsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<PagedResult<BoardDto>> Handle(SearchBoardsQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();


        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return new PagedResult<BoardDto>(
                items: Enumerable.Empty<BoardDto>(),
                totalCount: 0,
                page: request.PagedRequest.Page,
                pageSize: request.PagedRequest.PageSize);
        }

        var result = await uow.Boards.SearchAsync(
                    request.SearchTerm,
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
