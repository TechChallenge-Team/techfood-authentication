using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechFood.Domain.Entities;

namespace TechFood.Infra.Persistence.Contexts;

public class TechFoodContext(DbContextOptions<TechFoodContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;

    public async Task<bool> CommitAsync()
    {
        var success = await SaveChangesAsync() > 0;
        return success;
    }

    public Task RollbackAsync()
    {
        return Task.CompletedTask;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.LogTo(Console.WriteLine);
#endif
    }
}
