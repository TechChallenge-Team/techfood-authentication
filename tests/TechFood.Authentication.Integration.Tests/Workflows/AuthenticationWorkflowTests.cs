using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TechFood.Authentication.Application.Commands.ClientCredentials;
using TechFood.Authentication.Application.Commands.SignIn;
using TechFood.Authentication.Domain.Entities;
using TechFood.Authentication.Domain.Repositories;
using TechFood.Authentication.Domain.ValueObjects;
using TechFood.Authentication.Integration.Tests.Fixtures;

namespace TechFood.Authentication.Integration.Tests.Workflows;

public class AuthenticationWorkflowTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly IMediator _mediator;
    private readonly IServiceClientRepository _clientRepository;

    public AuthenticationWorkflowTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _mediator = _fixture.ServiceProvider.GetRequiredService<IMediator>();
        _clientRepository = _fixture.ServiceProvider.GetRequiredService<IServiceClientRepository>();
    }

    [Fact(DisplayName = "Should authenticate user and generate JWT token")]
    [Trait("Integration", "AuthenticationWorkflow")]
    public async Task UserAuthentication_WithValidCredentials_ShouldGenerateToken()
    {
        // Arrange
        var username = "testuser";
        var password = "Password123!";

        // Act
        var signInCommand = new SignInCommand(username, password);
        var result = await _mediator.Send(signInCommand);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.ExpiresIn.Should().BeGreaterThan(0);
        result.User.Should().NotBeNull();
        result.User.Username.Should().Be(username);
        result.User.Email.Should().Be("test@example.com");
        result.User.Role.Should().Be("Admin");
    }

    [Fact(DisplayName = "Should fail authentication with invalid password")]
    [Trait("Integration", "AuthenticationWorkflow")]
    public async Task UserAuthentication_WithInvalidPassword_ShouldThrowException()
    {
        // Arrange
        var username = "testuser";
        var wrongPassword = "WrongPassword123!";

        // Act & Assert
        var signInCommand = new SignInCommand(username, wrongPassword);
        await Assert.ThrowsAsync<Shared.Application.Exceptions.ApplicationException>(
            async () => await _mediator.Send(signInCommand));
    }

    [Fact(DisplayName = "Should fail authentication with non-existent user")]
    [Trait("Integration", "AuthenticationWorkflow")]
    public async Task UserAuthentication_WithNonExistentUser_ShouldThrowException()
    {
        // Arrange
        var username = "nonexistentuser";
        var password = "Password123!";

        // Act & Assert
        var signInCommand = new SignInCommand(username, password);
        await Assert.ThrowsAsync<Shared.Application.Exceptions.ApplicationException>(
            async () => await _mediator.Send(signInCommand));
    }

    [Fact(DisplayName = "Should generate client credentials token with valid credentials")]
    [Trait("Integration", "AuthenticationWorkflow")]
    public async Task ClientCredentials_WithValidCredentials_ShouldGenerateToken()
    {
        // Arrange
        var clientId = "test-client";
        var clientSecret = "ClientSecret123!";
        var scope = "read write";

        // Act
        var command = new ClientCredentialsCommand(clientId, clientSecret, scope);
        var result = await _mediator.Send(command);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.TokenType.Should().Be("Bearer");
        result.ExpiresIn.Should().BeGreaterThan(0);
        result.Scope.Should().Contain("read");
        result.Scope.Should().Contain("write");
    }

    [Fact(DisplayName = "Should fail client credentials with invalid secret")]
    [Trait("Integration", "AuthenticationWorkflow")]
    public async Task ClientCredentials_WithInvalidSecret_ShouldThrowException()
    {
        // Arrange
        var clientId = "test-client";
        var wrongSecret = "WrongSecret123!";

        // Act & Assert
        var command = new ClientCredentialsCommand(clientId, wrongSecret, null);
        await Assert.ThrowsAsync<Shared.Application.Exceptions.ApplicationException>(
            async () => await _mediator.Send(command));
    }

    [Fact(DisplayName = "Should fail client credentials with unauthorized scope")]
    [Trait("Integration", "AuthenticationWorkflow")]
    public async Task ClientCredentials_WithUnauthorizedScope_ShouldThrowException()
    {
        // Arrange
        var clientId = "test-client";
        var clientSecret = "ClientSecret123!";
        var unauthorizedScope = "admin";

        // Act & Assert
        var command = new ClientCredentialsCommand(clientId, clientSecret, unauthorizedScope);
        await Assert.ThrowsAsync<Shared.Application.Exceptions.ApplicationException>(
            async () => await _mediator.Send(command));
    }

    [Fact(DisplayName = "Should update client last used date after successful authentication")]
    [Trait("Integration", "AuthenticationWorkflow")]
    public async Task ClientCredentials_AfterSuccessfulAuth_ShouldUpdateLastUsed()
    {
        // Arrange
        var clientId = "test-client";
        var clientSecret = "ClientSecret123!";
        var clientBefore = await _clientRepository.GetByClientIdAsync(clientId);
        var lastUsedBefore = clientBefore!.LastUsedAt;

        await Task.Delay(100); // Pequeno delay para garantir diferença no timestamp

        // Act
        var command = new ClientCredentialsCommand(clientId, clientSecret, "read");
        await _mediator.Send(command);

        // Assert
        var clientAfter = await _clientRepository.GetByClientIdAsync(clientId);
        clientAfter!.LastUsedAt.Should().NotBeNull();
        if (lastUsedBefore.HasValue)
        {
            clientAfter.LastUsedAt.Should().BeAfter(lastUsedBefore.Value);
        }
    }

    [Fact(DisplayName = "Should create new user and authenticate successfully")]
    [Trait("Integration", "AuthenticationWorkflow")]
    public async Task CreateUserAndAuthenticate_ShouldSucceed()
    {
        // Arrange - Create new user
        var passwordHasher = new PasswordHasher<User>();
        var newUser = new User(
            new Name("New Test User"),
            "newuser",
            "User",
            new Email("newuser@example.com"));
        newUser.SetPassword(passwordHasher.HashPassword(newUser, "NewPassword123!"));

        _fixture.DbContext.Users.Add(newUser);
        await _fixture.DbContext.SaveChangesAsync();

        // Act - Authenticate
        var signInCommand = new SignInCommand("newuser", "NewPassword123!");
        var result = await _mediator.Send(signInCommand);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.User.Username.Should().Be("newuser");
        result.User.Email.Should().Be("newuser@example.com");
        result.User.Role.Should().Be("User");
    }
}
