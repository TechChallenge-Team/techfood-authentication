using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TechFood.Authentication.Application.Dto;
using TechFood.Authentication.Domain.Entities;
using TechFood.Authentication.Domain.Repositories;

namespace TechFood.Authentication.Application.Commands.ClientCredentials;

public class ClientCredentialsCommandHandler(
    IServiceClientRepository clientRepository,
    IConfiguration configuration)
        : IRequestHandler<ClientCredentialsCommand, TokenResultDto>
{
    private const string Issuer = "techfood-jwts";
    private const string Audience = "techfood";
    private static readonly TimeSpan _tokenExpiration = TimeSpan.FromMinutes(30);

    public async Task<TokenResultDto> Handle(ClientCredentialsCommand request, CancellationToken cancellationToken)
    {
        var client = await clientRepository.GetByClientIdAsync(request.ClientId);
        if (client == null || !client.IsActive)
        {
            throw new Shared.Application.Exceptions.ApplicationException("Invalid client credentials");
        }

        var passwordHasher = new PasswordHasher<ServiceClient>();
        var isValid = passwordHasher.VerifyHashedPassword(client, client.ClientSecretHash, request.ClientSecret);

        if (isValid == PasswordVerificationResult.Failed)
        {
            throw new Shared.Application.Exceptions.ApplicationException("Invalid client credentials");
        }

        var requestedScopes = request.Scope?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        var allowedScopes = client.Scopes;

        if (requestedScopes.Any())
        {
            var unauthorizedScopes = requestedScopes.Except(allowedScopes, StringComparer.OrdinalIgnoreCase).ToArray();
            if (unauthorizedScopes.Any())
            {
                throw new Shared.Application.Exceptions.ApplicationException($"Unauthorized scopes: {string.Join(", ", unauthorizedScopes)}");
            }
        }

        var grantedScopes = requestedScopes.Any() ? requestedScopes : allowedScopes;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, client.ClientId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("client_id", client.ClientId),
            new("client_name", client.Name),
            new("token_type", "service")
        };

        foreach (var scope in grantedScopes)
        {
            claims.Add(new("scope", scope));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.Add(_tokenExpiration),
            audience: Audience,
            issuer: Issuer,
            signingCredentials: creds);

        await clientRepository.UpdateLastUsedAsync(client.Id, DateTime.UtcNow);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResultDto(
            accessToken,
            "Bearer",
            (int)_tokenExpiration.TotalSeconds,
            string.Join(" ", grantedScopes)
        );
    }
}
