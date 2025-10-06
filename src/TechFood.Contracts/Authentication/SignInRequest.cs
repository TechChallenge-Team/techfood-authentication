using System.ComponentModel.DataAnnotations;

namespace TechFood.Contracts.Authentication;

public record SignInRequest(
    [Required] string Username,
    [Required] string Password);
