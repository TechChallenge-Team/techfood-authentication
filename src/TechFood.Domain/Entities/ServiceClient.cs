using System;
using System.Linq;
using TechFood.Shared.Domain.Entities;

namespace TechFood.Domain.Entities;

public class ServiceClient: Entity, IAggregateRoot
{
    public string ClientId { get; set; } = null!;

    public string ClientSecretHash { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string[] Scopes { get; set; } = [];

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastUsedAt { get; set; }

    public bool HasScope(string scope) => Scopes.Contains(scope, StringComparer.OrdinalIgnoreCase);
}
