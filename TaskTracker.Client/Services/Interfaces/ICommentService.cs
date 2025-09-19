using Refit;
using TaskTracker.Client.DTOs.Comment;

namespace TaskTracker.Client.Services.Interfaces;

public interface ICommentService
{
    [Get("/comment/{id}")]
    Task<CommentDto> GetByIdAsync(Guid id);


    [Get("/comment/card/{cardId}")]
    Task<List<CommentDto>> GetByCardIdAsync(Guid cardId);


    [Multipart]
    [Post("/comment")]
    Task<Guid> CreateAsync(
        [AliasAs("CardId")] string cardId,
        [AliasAs("UserId")] string userId,
        [AliasAs("Text")] string text,
        [AliasAs("CreatedBy")] string createdBy,
        [AliasAs("Files")] IEnumerable<StreamPart>? files
    );


    [Put("/comment/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateCommentDto command);

    
    [Delete("/comment/{id}")]
    Task DeleteAsync(Guid id);
}
