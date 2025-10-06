using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TechFood.Application.Authentication.Dto;
using TechFood.Domain.Entities;
using TechFood.Domain.Repositories;

namespace TechFood.Application.Authentication.Commands;

public class SignInCommandHandler(
        IUserRepository repo,
        IConfiguration configuration)
            : IRequestHandler<SignInCommand, SignInResultDto>
{
    private static readonly TimeSpan _tokenExpiration = TimeSpan.FromHours(1);

    public async Task<SignInResultDto> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var user = await repo.GetByUsernameOrEmailAsync(request.Username);
        if (user == null)
        {
            throw new Common.Exceptions.ApplicationException(Common.Resources.Exceptions.Auth_InvalidUseOrPassword);
        }

        var validation = new PasswordHasher<User>();
        var isValid = validation.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (isValid == PasswordVerificationResult.Failed)
        {
            throw new Common.Exceptions.ApplicationException(Common.Resources.Exceptions.Auth_InvalidUseOrPassword);
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: new[]
            {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.Role, user.Role)
            },
            expires: DateTime.UtcNow.Add(_tokenExpiration),
            audience: configuration["Jwt:Audience"],
            issuer: configuration["Jwt:Issuer"],
            signingCredentials: creds);

        return new(
            new JwtSecurityTokenHandler().WriteToken(token),
            null!,
            (int)_tokenExpiration.TotalSeconds,
            new(
                user.Id,
                user.Username,
                user.Name.FullName,
                user.Email?.Address,
                user.Role)
            );
    }
}
