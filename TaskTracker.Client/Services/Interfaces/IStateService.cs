using Refit;
using TaskTracker.Client.DTOs.State;
namespace TaskTracker.Client.Services.Interfaces;

public interface IStateService
{
    [Get("/state/{id}")]
    Task<StateDto> GetByIdAsync(Guid id);

    [Get("/state/card/{cardId}")]
    Task<StateDto> GetByCardIdAsync(Guid cardId);

    [Post("/state")]
    Task CreateAsync([Body] CreateStateDto command);

    [Put("/state/{cardId}")]
    Task UpdateAsync(Guid cardId, [Body] UpdateStateDto command);

    [Delete("/state/{id}")]
    Task DeleteAsync(Guid id);
}
