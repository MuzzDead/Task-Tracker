using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Comment.Queries.GetByCardId;

public class GetCommentsByCardIdQueryHandler : IRequestHandler<GetCommentsByCardIdQuery, IEnumerable<CommentDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    public GetCommentsByCardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CommentDto>> Handle(GetCommentsByCardIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var comments = await uow.Comments.GetByCardId(request.CardId);

        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }
}
