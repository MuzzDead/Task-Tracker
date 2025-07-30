using Refit;
using TaskTracker.Client.DTOs.User;

namespace TaskTracker.Client.Services.Interfaces;

public interface IUserService
{
    [Get("/user/{id}")]
    Task<UserDto> GetByIdAsync(Guid id);

    [Get("/user/by-email")]
    Task<UserDto> GetByEmailAsync([AliasAs("email")] string email);

    [Put("/user/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateUserDto model);

    [Delete("/user/{id}")]
    Task DeleteAsync(Guid id);
}
