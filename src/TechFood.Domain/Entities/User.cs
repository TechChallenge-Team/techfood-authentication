using TechFood.Domain.Common.Entities;
using TechFood.Domain.Common.Validations;
using TechFood.Domain.ValueObjects;

namespace TechFood.Domain.Entities;

public class User : Entity, IAggregateRoot
{
    private User() { }

    public User(
        Name name,
        string username,
        string role,
        Email? email)
    {
        Name = name;
        Username = username;
        Role = role;
        Email = email;
    }

    public Name Name { get; private set; } = null!;

    public string Username { get; private set; } = null!;

    public Email? Email { get; private set; }

    public string PasswordHash { get; private set; } = null!;

    public string Role { get; private set; } = null!;

    public void SetPassword(string passwordHash)
    {
        Validations.ThrowIfEmpty(passwordHash, Resources.Exceptions.User_PasswordHashIsEmpty);

        PasswordHash = passwordHash;
    }

    public void SetRole(string role)
    {
        Validations.ThrowIfEmpty(role, Resources.Exceptions.User_RoleIsEmpty);

        Role = role;
    }
}
