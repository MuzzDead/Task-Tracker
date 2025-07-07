using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.BoardRole.Queries.GetById;

public class GetBoardRoleByIdQueryHandler : IRequestHandler<GetBoardRoleByIdQuery, BoardRoleDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    public GetBoardRoleByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<BoardRoleDto> Handle(GetBoardRoleByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var role = await uow.BoardRoles.GetByIdAsync(request.Id);
        if (role == null)
        {
            throw new NotFoundException($"BoardRole with ID {request.Id} was not found.");
        }

        return _mapper.Map<BoardRoleDto>(role);
    }
}
