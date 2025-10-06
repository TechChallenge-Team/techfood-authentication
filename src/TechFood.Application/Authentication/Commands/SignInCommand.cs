using MediatR;
using TechFood.Application.Authentication.Dto;

namespace TechFood.Application.Authentication.Commands;

public record SignInCommand(string Username, string Password) : IRequest<SignInResultDto>;
