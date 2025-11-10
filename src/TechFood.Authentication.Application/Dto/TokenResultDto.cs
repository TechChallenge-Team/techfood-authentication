namespace TechFood.Authentication.Application.Dto;

public record TokenResultDto(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string? Scope = null
);
