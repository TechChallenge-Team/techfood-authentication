using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechFood.Domain.Common.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);

    Task<IEnumerable<T>> GetAllAsync();

    Task<Guid> AddAsync(T entity);

    Task DeleteAsync(T entity);
}
