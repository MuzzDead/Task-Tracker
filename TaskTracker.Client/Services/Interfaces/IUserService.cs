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

    [Put("/user/{id}/change-password")]
    Task ChangePasswordAsync(Guid id, [Body] ChangePasswordDto model);

    [Multipart]
    [Post("/user/{id}/avatar")]

    Task<UserDto> UploadAvatarAsync(Guid id, [AliasAs("avatar")] StreamPart avatar);

    [Delete("/user/{id}/avatar")]
    Task<UserDto> DeleteAvatarAsync(Guid id);


    [Get("/user/{id}/avatar")]
    Task<Stream> GetAvatarAsync(Guid id);
}
