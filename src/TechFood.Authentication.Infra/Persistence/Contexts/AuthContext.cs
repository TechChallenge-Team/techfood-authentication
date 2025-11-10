using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechFood.Authentication.Domain.Entities;
using TechFood.Shared.Infra.Persistence.Contexts;

namespace TechFood.Authentication.Infra.Persistence.Contexts;

public class AuthContext(DbContextOptions<AuthContext> options) : TechFoodContext(options)
{
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<ServiceClient> ServiceClients { get; set; } = null!;

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
        // Seed User
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

        modelBuilder.Entity<ServiceClient>()
            .HasData(
                // client secret: iY0M+6c/1fV9cgMV2HE6uIcpeiW5uMQpdEuff4t6siE=
                new
                {
                    Id = new Guid("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d"),
                    ClientId = "order-service",
                    ClientSecretHash = "AQAAAAIAAYagAAAAEGc5qkWBdWL8/9BHGSyC24vz49eu41YPzSg0AaUFWNzA/qJgKZQy0dK+BiUDwo45qw==",
                    Name = "Order Service",
                    Scopes = new[] { "orders.read", "orders.write", "users.read" },
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                },
                // client secret: SLll0AcTiwcqMqqo2IJOGoHHjsVGoynb0ECIebV0yow=
                new
                {
                    Id = new Guid("b2c3d4e5-f6a7-4b5c-9d0e-1f2a3b4c5d6e"),
                    ClientId = "payment-service",
                    ClientSecretHash = "AQAAAAIAAYagAAAAEJ1h6kwzl70PTafDv/XTtOK0OD5fgj+7ushepfeOcmT2fagJajeYNh0jeQSo7YYIlQ==",
                    Name = "Payment Service",
                    Scopes = new[] { "payments.read", "payments.write", "orders.read", "users.read" },
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                },
                // client secret: tTBXfwMODoJno6VcUYHYhaILa0a8aZ2YLByafZTgjJU=
                new
                {
                    Id = new Guid("c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f"),
                    ClientId = "kitchen-service",
                    ClientSecretHash = "AAQAAAAIAAYagAAAAEK0Ly9jwdR3uEE1dXSiXeN6Zqpnvz2XWdEKTcaoc+MBGSvoj31/sGh4wlH3WEggI5Q==",
                    Name = "Kitchen Service",
                    Scopes = new[] { "kitchen.read", "kitchen.write", "orders.read", "users.read" },
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                },
                // client secret: H4Ebf8wN59GG1y0REozqTFlQKybLbsiOigoSvRWBgiA=
                new
                {
                    Id = new Guid("d4e5f6a7-b8c9-4d5e-1f2a-3b4c5d6e7f8a"),
                    ClientId = "backoffice-service",
                    ClientSecretHash = "AQAAAAIAAYagAAAAEJlLak1GYPXjel9Lc9LhwpLilkyUvFhwBuxNL0g8sn6POZv3fD+XqHN0C90pJbE9vg==",
                    Name = "Backoffice",
                    Scopes = new[] { "orders.read", "orders.write", "payments.read", "payments.write", "users.read", "users.write", "kitchen.read", "kitchen.write" },
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false
                }
            );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.LogTo(Console.WriteLine);
#endif
    }
}
