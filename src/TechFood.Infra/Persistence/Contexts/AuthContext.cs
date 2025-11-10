using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechFood.Domain.Entities;
using TechFood.Shared.Infra.Persistence.Contexts;

namespace TechFood.Infra.Persistence.Contexts;

public class AuthContext(DbContextOptions<AuthContext> options) : TechFoodContext(options)
{
    public DbSet<User> Users { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthContext).Assembly);

        var properties = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetProperties());

        var stringProperties = properties.Where(p => p.ClrType == typeof(string));
        foreach (var property in stringProperties)
        {
            var maxLength = property.GetMaxLength() ?? 50;

            property.SetColumnType($"varchar({maxLength})");
        }

        var booleanProperties = properties
            .Where(p => p.ClrType == typeof(bool) ||
                        p.ClrType == typeof(bool?));

        foreach (var property in booleanProperties)
        {
            property.SetColumnType("bit");
            property.IsNullable = false;
        }

        var dateTimeProperties = properties.Where(p => p.ClrType == typeof(DateTime));

        foreach (var property in dateTimeProperties)
        {
            property.SetColumnType("datetime");
        }

        var enumProperties = properties.Where(p => p.ClrType == typeof(Enum));

        foreach (var property in enumProperties)
        {
            property.SetColumnType("smallint");
        }

        var amountProperties = properties
            .Where(p => p.ClrType == typeof(decimal) ||
                        p.ClrType == typeof(decimal?));

        foreach (var property in amountProperties)
        {
            property.SetColumnType("decimal(6, 2)");
        }

        SeedContext(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void SeedContext(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().OwnsOne(c => c.Name)
           .HasData(
               new { UserId = new Guid("fa09f3a0-f22d-40a8-9cca-0c64e5ed50e4"), FullName = "John Admin" }
           );

        modelBuilder.Entity<User>().OwnsOne(c => c.Email)
         .HasData(
             new { UserId = new Guid("fa09f3a0-f22d-40a8-9cca-0c64e5ed50e4"), Address = "john.admin@techfood.com" }
         );

        modelBuilder.Entity<User>()
            .HasData(
                // password: 123456
                new { Id = new Guid("fa09f3a0-f22d-40a8-9cca-0c64e5ed50e4"), Username = "john.admin", Role = "admin", PasswordHash = "AQAAAAIAAYagAAAAEKs0I0Zk5QKKieJTm20PwvTmpkSfnp5BhSl5E35ny8DqffCJA+CiDRnnKRCeOx8+mg==", IsDeleted = false }
            );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.LogTo(Console.WriteLine);
#endif
    }
}
