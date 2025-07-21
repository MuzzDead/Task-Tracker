using Refit;
using TaskTracker.Client.DTOs.Card;

namespace TaskTracker.Client.Services.Interfaces;

public interface ICardService
{
    [Post("/api/card")]
    Task<CardDto> CreateAsync([Body] CreateCardDto command);

    
    [Put("/api/card/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateCardDto command);

    
    [Delete("/api/card/{id}")]
    Task DeleteAsync(Guid id);

    
    [Get("/api/card/{id}")]
    Task<CardDto> GetByIdAsync(Guid id);

    
    [Get("/api/card/column/{columnId}")]
    Task<List<CardDto>> GetByColumnIdAsync(Guid columnId);
}
