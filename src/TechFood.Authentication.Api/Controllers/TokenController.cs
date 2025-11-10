using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechFood.Authentication.Application.Commands.ClientCredentials;
using TechFood.Authentication.Contracts.Authentication;

namespace TechFood.Authentication.Controllers;

[ApiController]
[Route("v1/[controller]")]
[AllowAnonymous]
public class TokenController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Endpoint para obtenção de token usando Client Credentials (OAuth 2.0)
    /// Para autenticação entre serviços (machine-to-machine)
    /// </summary>
    /// <param name="request">Credenciais do client</param>
    /// <returns>Access token JWT</returns>
    /// <response code="200">Token gerado com sucesso</response>
    /// <response code="400">Request inválido</response>
    /// <response code="401">Credenciais inválidas</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest request)
    {
        if (request.GrantType != "client_credentials")
        {
            return BadRequest(new { error = "unsupported_grant_type", error_description = "Only 'client_credentials' grant type is supported" });
        }

        if (string.IsNullOrWhiteSpace(request.ClientId) || string.IsNullOrWhiteSpace(request.ClientSecret))
        {
            return BadRequest(new { error = "invalid_request", error_description = "client_id and client_secret are required" });
        }

        var command = new ClientCredentialsCommand(
            request.ClientId,
            request.ClientSecret,
            request.Scope
        );

        var result = await _mediator.Send(command);

        return Ok(new
        {
            access_token = result.AccessToken,
            token_type = result.TokenType,
            expires_in = result.ExpiresIn,
            scope = result.Scope
        });
    }
}
