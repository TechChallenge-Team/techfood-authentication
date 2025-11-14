using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TechFood.Authentication.Application.Commands.ClientCredentials;
using TechFood.Authentication.Domain.Entities;
using TechFood.Authentication.Domain.Repositories;

namespace TechFood.Authentication.Application.Tests.Commands;

public class ClientCredentialsCommandHandlerTests
{
    private readonly Mock<IServiceClientRepository> _clientRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly ClientCredentialsCommandHandler _handler;

    public ClientCredentialsCommandHandlerTests()
    {
        _clientRepositoryMock = new Mock<IServiceClientRepository>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock
            .Setup(x => x["Authentication:Jwt:Key"])
            .Returns("super-secret-key-for-testing-purposes-minimum-32-chars");

        _handler = new ClientCredentialsCommandHandler(
            _clientRepositoryMock.Object,
            _configurationMock.Object);
    }

    [Fact(DisplayName = "Should generate token with valid client credentials")]
    [Trait("Application", "ClientCredentialsCommandHandler")]
    public async Task Handle_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var clientId = "test-service";
        var clientSecret = "SecretKey123!";
        var passwordHasher = new PasswordHasher<ServiceClient>();

        var client = new ServiceClient
        {
            ClientId = clientId,
            Name = "Test Service",
            Scopes = new[] { "read", "write" },
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        client.ClientSecretHash = passwordHasher.HashPassword(client, clientSecret);

        var command = new ClientCredentialsCommand(clientId, clientSecret, "read write");

        _clientRepositoryMock
            .Setup(x => x.GetByClientIdAsync(clientId))
            .ReturnsAsync(client);

        _clientRepositoryMock
            .Setup(x => x.UpdateLastUsedAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.TokenType.Should().Be("Bearer");
        result.ExpiresIn.Should().BeGreaterThan(0);
        result.Scope.Should().Be("read write");

        _clientRepositoryMock.Verify(x => x.UpdateLastUsedAsync(client.Id, It.IsAny<DateTime>()), Times.Once);
    }

    [Fact(DisplayName = "Should throw exception when client not found")]
    [Trait("Application", "ClientCredentialsCommandHandler")]
    public async Task Handle_WithInvalidClientId_ShouldThrowException()
    {
        // Arrange
        var command = new ClientCredentialsCommand("nonexistent", "secret", null);

        _clientRepositoryMock
            .Setup(x => x.GetByClientIdAsync("nonexistent"))
            .ReturnsAsync((ServiceClient)null!);

        // Act & Assert
        await Assert.ThrowsAsync<Shared.Application.Exceptions.ApplicationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw exception when client is inactive")]
    [Trait("Application", "ClientCredentialsCommandHandler")]
    public async Task Handle_WithInactiveClient_ShouldThrowException()
    {
        // Arrange
        var client = new ServiceClient
        {
            ClientId = "test-client",
            IsActive = false
        };

        var command = new ClientCredentialsCommand("test-client", "secret", null);

        _clientRepositoryMock
            .Setup(x => x.GetByClientIdAsync("test-client"))
            .ReturnsAsync(client);

        // Act & Assert
        await Assert.ThrowsAsync<Shared.Application.Exceptions.ApplicationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw exception when client secret is invalid")]
    [Trait("Application", "ClientCredentialsCommandHandler")]
    public async Task Handle_WithInvalidClientSecret_ShouldThrowException()
    {
        // Arrange
        var passwordHasher = new PasswordHasher<ServiceClient>();
        var client = new ServiceClient
        {
            ClientId = "test-client",
            IsActive = true
        };
        client.ClientSecretHash = passwordHasher.HashPassword(client, "CorrectSecret");

        var command = new ClientCredentialsCommand("test-client", "WrongSecret", null);

        _clientRepositoryMock
            .Setup(x => x.GetByClientIdAsync("test-client"))
            .ReturnsAsync(client);

        // Act & Assert
        await Assert.ThrowsAsync<Shared.Application.Exceptions.ApplicationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw exception when requesting unauthorized scopes")]
    [Trait("Application", "ClientCredentialsCommandHandler")]
    public async Task Handle_WithUnauthorizedScopes_ShouldThrowException()
    {
        // Arrange
        var passwordHasher = new PasswordHasher<ServiceClient>();
        var client = new ServiceClient
        {
            ClientId = "test-client",
            IsActive = true,
            Scopes = new[] { "read" }
        };
        client.ClientSecretHash = passwordHasher.HashPassword(client, "secret");

        var command = new ClientCredentialsCommand("test-client", "secret", "read write admin");

        _clientRepositoryMock
            .Setup(x => x.GetByClientIdAsync("test-client"))
            .ReturnsAsync(client);

        // Act & Assert
        await Assert.ThrowsAsync<Shared.Application.Exceptions.ApplicationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should grant all allowed scopes when no specific scope requested")]
    [Trait("Application", "ClientCredentialsCommandHandler")]
    public async Task Handle_WithoutRequestedScopes_ShouldGrantAllAllowedScopes()
    {
        // Arrange
        var passwordHasher = new PasswordHasher<ServiceClient>();
        var client = new ServiceClient
        {
            ClientId = "test-client",
            Name = "Test Client",
            IsActive = true,
            Scopes = new[] { "read", "write", "delete" }
        };
        client.ClientSecretHash = passwordHasher.HashPassword(client, "secret");

        var command = new ClientCredentialsCommand("test-client", "secret", null);

        _clientRepositoryMock
            .Setup(x => x.GetByClientIdAsync("test-client"))
            .ReturnsAsync(client);

        _clientRepositoryMock
            .Setup(x => x.UpdateLastUsedAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Scope.Should().Contain("read");
        result.Scope.Should().Contain("write");
        result.Scope.Should().Contain("delete");
    }
}
