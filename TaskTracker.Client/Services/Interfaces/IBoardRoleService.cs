using Refit;
using TaskTracker.Client.DTOs.Member;

namespace TaskTracker.Client.Services.Interfaces;

public interface IBoardRoleService
{
    [Get("/boardrole")]
    Task<IEnumerable<BoardRoleDto>> GetAllAsync();

    [Get("/boardrole/{id}")]
    Task<BoardRoleDto> GetByIdAsync(Guid id);

    [Post("/boardrole")]
    Task<Guid> CreateAsync([Body] CreateBoardRoleDto command);

    [Put("/boardrole/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateBoardRoleDto command);

    [Delete("/boardrole/{id}")]
    Task DeleteAsync(Guid id);

    [Get("/boardrole/members/{boardId}")]
    Task<IEnumerable<MemberDto>> GetMemberByBoardIdAsync(Guid boardId);
}
