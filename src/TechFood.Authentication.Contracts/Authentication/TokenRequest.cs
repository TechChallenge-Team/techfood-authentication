namespace TechFood.Authentication.Contracts.Authentication;

public record TokenRequest
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string GrantType { get; init; } = "client_credentials";
    public string? Scope { get; init; }
}
