namespace TechFood.Authentication.Contracts.Authentication;

public record TokenRequest
{
    public string ClientId { get; init; } = null!;

    public string ClientSecret { get; init; } = null!;

    public string GrantType { get; init; } = "client_credentials";

    public string? Scope { get; init; }
}
