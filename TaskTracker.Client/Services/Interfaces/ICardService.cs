using Refit;
using TaskTracker.Client.DTOs.Card;

namespace TaskTracker.Client.Services.Interfaces;

public interface ICardService
{
    [Post("/card")]
    Task<Guid> CreateAsync([Body] CreateCardDto command);


    [Put("/card/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateCardDto command);


    [Delete("/card/{id}")]
    Task DeleteAsync(Guid id);


    [Get("/card/{id}")]
    Task<CardDto> GetByIdAsync(Guid id);


    [Get("/card/column/{columnId}")]
    Task<List<CardDto>> GetByColumnIdAsync(Guid columnId);
}

