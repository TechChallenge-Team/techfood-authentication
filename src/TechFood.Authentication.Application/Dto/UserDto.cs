using System;

namespace TechFood.Authentication.Application.Dto;

public record UserDto(Guid Id, string Name, string Username, string? Email, string Role);
