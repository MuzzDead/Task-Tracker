using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.BoardRole.Queries.GetMembersByBoardId;

public class GetMembersByBoardIdQueryHandler : IRequestHandler<GetMembersByBoardIdQuery, IEnumerable<MemberDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public GetMembersByBoardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<IEnumerable<MemberDto>> Handle(GetMembersByBoardIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var members = await uow.BoardRoles.GetMembersByBoardIdAsync(request.BoardId);
        if (members == null)
        {
            throw new NotFoundException($"Members with BoardId {request.BoardId} was not found.");
        }

        return members;
    }
}
