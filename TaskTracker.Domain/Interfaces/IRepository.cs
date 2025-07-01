using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(Guid Id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task Update(T entity);
    Task Remove(Guid Id);
}
