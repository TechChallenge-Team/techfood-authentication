using System;
using System.Threading.Tasks;
using TechFood.Domain.Entities;

namespace TechFood.Domain.Repositories;

public interface IServiceClientRepository
{
    Task<ServiceClient?> GetByClientIdAsync(string clientId);
    Task UpdateLastUsedAsync(Guid id, DateTime lastUsedAt);
}
