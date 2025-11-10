using MediatR;
using TechFood.Application.Dto;

namespace TechFood.Application.Commands.ClientCredentials;

public record ClientCredentialsCommand(
    string ClientId,
    string ClientSecret,
    string? Scope = null
) : IRequest<TokenResultDto>;
