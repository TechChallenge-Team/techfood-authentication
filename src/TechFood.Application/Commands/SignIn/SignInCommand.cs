using MediatR;
using TechFood.Application.Dto;

namespace TechFood.Application.Commands.SignIn;

public record SignInCommand(string Username, string Password) : IRequest<SignInResultDto>;
