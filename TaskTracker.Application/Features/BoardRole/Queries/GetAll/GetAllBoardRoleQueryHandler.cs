using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.BoardRole.Queries.GetAll;

public class GetAllBoardRoleQueryHandler : IRequestHandler<GetAllBoardRoleQuery, IEnumerable<BoardRoleDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    public GetAllBoardRoleQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BoardRoleDto>> Handle(GetAllBoardRoleQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var roles = await uow.BoardRoles.GetAllAsync();

        return _mapper.Map<IEnumerable<BoardRoleDto>>(roles);
    }
}
