using System.ComponentModel.DataAnnotations;

namespace TechFood.Authentication.Contracts.Authentication;

public record SignInRequest(
    [Required] string Username,
    [Required] string Password);
