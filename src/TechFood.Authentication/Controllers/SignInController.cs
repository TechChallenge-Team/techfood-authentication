using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechFood.Application.Commands.SignIn;
using TechFood.Contracts.Authentication;

namespace TechFood.Authentication.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class SignInController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> SignInAsync(SignInRequest request)
    {
        var command = new SignInCommand(request.Username, request.Password);

        var authResult = await _mediator.Send(command);

        return Ok(authResult);
    }
}
