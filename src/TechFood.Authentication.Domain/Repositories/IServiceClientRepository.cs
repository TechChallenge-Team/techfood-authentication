using System;
using System.Threading.Tasks;
using TechFood.Authentication.Domain.Entities;

namespace TechFood.Authentication.Domain.Repositories;

public interface IServiceClientRepository
{
    Task<ServiceClient?> GetByClientIdAsync(string clientId);
    Task UpdateLastUsedAsync(Guid id, DateTime lastUsedAt);
}
