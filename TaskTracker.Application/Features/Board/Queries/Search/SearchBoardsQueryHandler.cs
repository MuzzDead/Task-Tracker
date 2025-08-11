using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Board.Queries.Search;

public class SearchBoardsQueryHandler : IRequestHandler<SearchBoardsQuery, IEnumerable<BoardDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;

    public SearchBoardsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BoardDto>> Handle(SearchBoardsQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();
        
        
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return Enumerable.Empty<BoardDto>();

        var boards = await uow.Boards.SearchAsync(request.SearchTerm, request.UserId);

        return _mapper.Map<IEnumerable<BoardDto>>(boards);
    }
}
