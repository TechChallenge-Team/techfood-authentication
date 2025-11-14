using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechFood.Authentication.Controllers;
using TechFood.Authentication.Application.Commands.ClientCredentials;
using TechFood.Authentication.Application.Dto;
using TechFood.Authentication.Contracts.Authentication;

namespace TechFood.Authentication.Api.Tests.Controllers;

public class TokenControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TokenController _controller;

    public TokenControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new TokenController(_mediatorMock.Object);
    }

    [Fact(DisplayName = "GetTokenAsync should return Ok with token result")]
    [Trait("Api", "TokenController")]
    public async Task GetTokenAsync_WithValidCredentials_ShouldReturnOkWithToken()
    {
        // Arrange
        var request = new TokenRequest
        {
            GrantType = "client_credentials",
            ClientId = "test-client",
            ClientSecret = "test-secret",
            Scope = "read write"
        };

        var expectedResult = new TokenResultDto(
            "jwt-token-here",
            "Bearer",
            1800,
            "read write");

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<ClientCredentialsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetTokenAsync(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(expectedResult);

        _mediatorMock.Verify(x => x.Send(
            It.Is<ClientCredentialsCommand>(cmd =>
                cmd.ClientId == "test-client" &&
                cmd.ClientSecret == "test-secret" &&
                cmd.Scope == "read write"),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "GetTokenAsync should return BadRequest for unsupported grant type")]
    [Trait("Api", "TokenController")]
    public async Task GetTokenAsync_WithUnsupportedGrantType_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new TokenRequest
        {
            GrantType = "password",
            ClientId = "test-client",
            ClientSecret = "test-secret"
        };

        // Act
        var result = await _controller.GetTokenAsync(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.Should().NotBeNull();

        _mediatorMock.Verify(x => x.Send(
            It.IsAny<ClientCredentialsCommand>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "GetTokenAsync should return BadRequest when ClientId is missing")]
    [Trait("Api", "TokenController")]
    public async Task GetTokenAsync_WithMissingClientId_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new TokenRequest
        {
            GrantType = "client_credentials",
            ClientId = "",
            ClientSecret = "test-secret"
        };

        // Act
        var result = await _controller.GetTokenAsync(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();

        _mediatorMock.Verify(x => x.Send(
            It.IsAny<ClientCredentialsCommand>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "GetTokenAsync should return BadRequest when ClientSecret is missing")]
    [Trait("Api", "TokenController")]
    public async Task GetTokenAsync_WithMissingClientSecret_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new TokenRequest
        {
            GrantType = "client_credentials",
            ClientId = "test-client",
            ClientSecret = ""
        };

        // Act
        var result = await _controller.GetTokenAsync(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();

        _mediatorMock.Verify(x => x.Send(
            It.IsAny<ClientCredentialsCommand>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "GetTokenAsync should work without scope")]
    [Trait("Api", "TokenController")]
    public async Task GetTokenAsync_WithoutScope_ShouldReturnOkWithToken()
    {
        // Arrange
        var request = new TokenRequest
        {
            GrantType = "client_credentials",
            ClientId = "test-client",
            ClientSecret = "test-secret",
            Scope = null
        };

        var expectedResult = new TokenResultDto(
            "jwt-token-here",
            "Bearer",
            1800,
            "");

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<ClientCredentialsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetTokenAsync(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(expectedResult);
    }
}
