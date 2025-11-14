using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TechFood.Authentication.Domain.Entities;
using TechFood.Authentication.Domain.ValueObjects;
using TechFood.Authentication.Infra.Persistence.Contexts;
using TechFood.Shared.Domain.UoW;
using TechFood.Shared.Infra.Persistence.UoW;

namespace TechFood.Authentication.Integration.Tests.Fixtures;

public class IntegrationTestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }

    public AuthContext DbContext { get; }

    public IntegrationTestFixture()
    {
        var services = new ServiceCollection();

        // Configure in-memory database
        services.AddDbContext<AuthContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

        // Configure Unit of Work
        services.TryAddScoped<IUnitOfWorkTransaction, UnitOfWorkTransaction>();
        services.TryAddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<AuthContext>());

        // Configure IConfiguration for JWT settings
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Authentication:Jwt:Key"] = "super-secret-key-for-testing-purposes-minimum-32-chars-long-key"
            }!)
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Register application services
        services.AddMediatR(typeof(Application.Commands.SignIn.SignInCommand).Assembly);

        // Register domain repositories
        services.AddScoped<Domain.Repositories.IUserRepository, Infra.Persistence.Repositories.UserRepository>();
        services.AddScoped<Domain.Repositories.IServiceClientRepository, Infra.Persistence.Repositories.ServiceClientRepository>();

        ServiceProvider = services.BuildServiceProvider();
        DbContext = ServiceProvider.GetRequiredService<AuthContext>();

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var passwordHasher = new PasswordHasher<User>();
        var clientPasswordHasher = new PasswordHasher<ServiceClient>();

        // Seed test user
        var testUser = new User(
            new Name("Test User"),
            "testuser",
            "Admin",
            new Email("test@example.com"));
        testUser.SetPassword(passwordHasher.HashPassword(testUser, "Password123!"));

        DbContext.Users.Add(testUser);

        // Seed test service client
        var testClient = new ServiceClient
        {
            ClientId = "test-client",
            Name = "Test Service Client",
            Scopes = new[] { "read", "write", "delete" },
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        testClient.ClientSecretHash = clientPasswordHasher.HashPassword(testClient, "ClientSecret123!");

        DbContext.ServiceClients.Add(testClient);

        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        DbContext?.Database.EnsureDeleted();
        DbContext?.Dispose();

        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
