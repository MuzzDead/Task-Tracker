using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Features.Board.Queries.GetByUserId;

public class GetBoardsByUserIdQueryHandler : IRequestHandler<GetBoardsByUserIdQuery, IEnumerable<BoardDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    public GetBoardsByUserIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetBoardsByUserIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var boards = await uow.Boards.GetByUserId(request.UserId);

        return boards == null ? null : _mapper.Map<IEnumerable<BoardDto>>(boards);
    }
}
