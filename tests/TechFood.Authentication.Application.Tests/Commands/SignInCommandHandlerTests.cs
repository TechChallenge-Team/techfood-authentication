using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TechFood.Authentication.Application.Commands.SignIn;
using TechFood.Authentication.Domain.Entities;
using TechFood.Authentication.Domain.Repositories;
using TechFood.Authentication.Domain.ValueObjects;

namespace TechFood.Authentication.Application.Tests.Commands;

public class SignInCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly SignInCommandHandler _handler;

    public SignInCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock
            .Setup(x => x["Authentication:Jwt:Key"])
            .Returns("super-secret-key-for-testing-purposes-minimum-32-chars");

        _handler = new SignInCommandHandler(
            _userRepositoryMock.Object,
            _configurationMock.Object);
    }

    [Fact(DisplayName = "Should authenticate user with valid credentials")]
    [Trait("Application", "SignInCommandHandler")]
    public async Task Handle_WithValidCredentials_ShouldReturnSignInResult()
    {
        // Arrange
        var username = "testuser";
        var password = "Password123!";
        var passwordHasher = new PasswordHasher<User>();

        var user = new User(
            new Name("John Doe"),
            username,
            "Admin",
            new Email("john@example.com"));

        user.SetPassword(passwordHasher.HashPassword(user, password));

        var command = new SignInCommand(username, password);

        _userRepositoryMock
            .Setup(x => x.GetByUsernameOrEmailAsync(username))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.ExpiresIn.Should().BeGreaterThan(0);
        result.User.Should().NotBeNull();
        result.User.Username.Should().Be(username);
        result.User.Name.Should().Be("John Doe");
        result.User.Email.Should().Be("john@example.com");
        result.User.Role.Should().Be("Admin");
    }

    [Fact(DisplayName = "Should throw exception when user not found")]
    [Trait("Application", "SignInCommandHandler")]
    public async Task Handle_WithInvalidUsername_ShouldThrowException()
    {
        // Arrange
        var command = new SignInCommand("nonexistent", "password");

        _userRepositoryMock
            .Setup(x => x.GetByUsernameOrEmailAsync("nonexistent"))
            .ReturnsAsync((User)null!);

        // Act & Assert
        await Assert.ThrowsAsync<Shared.Application.Exceptions.ApplicationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw exception when password is invalid")]
    [Trait("Application", "SignInCommandHandler")]
    public async Task Handle_WithInvalidPassword_ShouldThrowException()
    {
        // Arrange
        var username = "testuser";
        var correctPassword = "CorrectPassword123!";
        var wrongPassword = "WrongPassword123!";
        var passwordHasher = new PasswordHasher<User>();

        var user = new User(
            new Name("John Doe"),
            username,
            "Admin",
            new Email("john@example.com"));

        user.SetPassword(passwordHasher.HashPassword(user, correctPassword));

        var command = new SignInCommand(username, wrongPassword);

        _userRepositoryMock
            .Setup(x => x.GetByUsernameOrEmailAsync(username))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<Shared.Application.Exceptions.ApplicationException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }
}
