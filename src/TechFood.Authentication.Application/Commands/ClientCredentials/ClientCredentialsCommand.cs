using MediatR;
using TechFood.Authentication.Application.Dto;

namespace TechFood.Authentication.Application.Commands.ClientCredentials;

public record ClientCredentialsCommand(
    string ClientId,
    string ClientSecret,
    string? Scope = null
) : IRequest<TokenResultDto>;
