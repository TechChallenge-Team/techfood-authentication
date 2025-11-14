using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechFood.Authentication.Controllers;
using TechFood.Authentication.Application.Commands.SignIn;
using TechFood.Authentication.Application.Dto;
using TechFood.Authentication.Contracts.Authentication;

namespace TechFood.Authentication.Api.Tests.Controllers;

public class SignInControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly SignInController _controller;

    public SignInControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new SignInController(_mediatorMock.Object);
    }

    [Fact(DisplayName = "SignInAsync should return Ok with auth result")]
    [Trait("Api", "SignInController")]
    public async Task SignInAsync_WithValidCredentials_ShouldReturnOkWithAuthResult()
    {
        // Arrange
        var request = new SignInRequest("testuser", "password123");
        var userDto = new UserDto(Guid.NewGuid(), "Test User", "testuser", "test@example.com", "User");
        var expectedResult = new SignInResultDto("jwt-token-here", null!, 3600, userDto);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<SignInCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.SignInAsync(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(expectedResult);

        _mediatorMock.Verify(x => x.Send(
            It.Is<SignInCommand>(cmd =>
                cmd.Username == "testuser" &&
                cmd.Password == "password123"),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "SignInAsync should forward command to mediator")]
    [Trait("Api", "SignInController")]
    public async Task SignInAsync_ShouldForwardCommandToMediator()
    {
        // Arrange
        var username = "user@example.com";
        var password = "SecureP@ss123";
        var request = new SignInRequest(username, password);
        var userDto = new UserDto(Guid.NewGuid(), "User Name", username, username, "User");
        var expectedResult = new SignInResultDto("token", null!, 3600, userDto);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<SignInCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.SignInAsync(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mediatorMock.Verify(x => x.Send(
            It.Is<SignInCommand>(cmd => cmd.Username == username && cmd.Password == password),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
