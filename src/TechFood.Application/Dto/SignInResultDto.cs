namespace TechFood.Application.Dto;

public record SignInResultDto(string AccessToken, string RefreshToken, int ExpiresIn, UserDto User);
