using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechFood.Domain.Entities;
using TechFood.Domain.Repositories;
using TechFood.Infra.Persistence.Contexts;

namespace TechFood.Infra.Persistence.Repositories;

public class ServiceClientRepository(AuthContext context) : IServiceClientRepository
{
    private readonly AuthContext _context = context;

    public async Task<ServiceClient?> GetByClientIdAsync(string clientId)
    {
        return await _context.ServiceClients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClientId == clientId);
    }

    public async Task UpdateLastUsedAsync(Guid id, DateTime lastUsedAt)
    {
        var client = await _context.ServiceClients.FindAsync(id);
        if (client != null)
        {
            client.LastUsedAt = lastUsedAt;
            await _context.SaveChangesAsync();
        }
    }
}
