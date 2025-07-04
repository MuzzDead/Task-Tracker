namespace TaskTracker.Application.Common.Interfaces.Base;

public interface IRepository<TEntity, TId> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TId Id);
    Task<IEnumerable<TEntity>> GetAllAsync();

    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TId Id);
}
