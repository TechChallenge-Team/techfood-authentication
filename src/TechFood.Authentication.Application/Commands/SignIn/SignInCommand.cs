using MediatR;
using TechFood.Authentication.Application.Dto;

namespace TechFood.Authentication.Application.Commands.SignIn;

public record SignInCommand(string Username, string Password) : IRequest<SignInResultDto>;
