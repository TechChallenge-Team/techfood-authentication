using FluentAssertions;
using TechFood.Authentication.Domain.Entities;

namespace TechFood.Authentication.Domain.Tests;

public class ServiceClientTests
{
    [Fact(DisplayName = "Should verify if client has scope")]
    [Trait("Domain", "ServiceClient")]
    public void HasScope_WithExistingScope_ShouldReturnTrue()
    {
        // Arrange
        var client = new ServiceClient
        {
            ClientId = "test-client",
            Name = "Test Client",
            Scopes = new[] { "read", "write", "delete" },
            IsActive = true
        };

        // Act
        var hasReadScope = client.HasScope("read");
        var hasWriteScope = client.HasScope("write");

        // Assert
        hasReadScope.Should().BeTrue();
        hasWriteScope.Should().BeTrue();
    }

    [Fact(DisplayName = "Should return false when client does not have scope")]
    [Trait("Domain", "ServiceClient")]
    public void HasScope_WithNonExistingScope_ShouldReturnFalse()
    {
        // Arrange
        var client = new ServiceClient
        {
            ClientId = "test-client",
            Name = "Test Client",
            Scopes = new[] { "read" },
            IsActive = true
        };

        // Act
        var hasAdminScope = client.HasScope("admin");

        // Assert
        hasAdminScope.Should().BeFalse();
    }

    [Fact(DisplayName = "Should check scope case-insensitively")]
    [Trait("Domain", "ServiceClient")]
    public void HasScope_WithDifferentCase_ShouldReturnTrue()
    {
        // Arrange
        var client = new ServiceClient
        {
            ClientId = "test-client",
            Name = "Test Client",
            Scopes = new[] { "READ", "WRITE" },
            IsActive = true
        };

        // Act
        var hasReadScope = client.HasScope("read");
        var hasWriteScope = client.HasScope("Write");

        // Assert
        hasReadScope.Should().BeTrue();
        hasWriteScope.Should().BeTrue();
    }

    [Fact(DisplayName = "Should create service client with properties")]
    [Trait("Domain", "ServiceClient")]
    public void ServiceClient_WithValidData_ShouldSetProperties()
    {
        // Arrange & Act
        var client = new ServiceClient
        {
            ClientId = "my-service",
            ClientSecretHash = "hashed_secret",
            Name = "My Service",
            Scopes = new[] { "api:read", "api:write" },
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LastUsedAt = null
        };

        // Assert
        client.ClientId.Should().Be("my-service");
        client.Name.Should().Be("My Service");
        client.Scopes.Should().HaveCount(2);
        client.IsActive.Should().BeTrue();
        client.LastUsedAt.Should().BeNull();
    }

    [Fact(DisplayName = "Should track last used date")]
    [Trait("Domain", "ServiceClient")]
    public void ServiceClient_LastUsedAt_ShouldBeTrackable()
    {
        // Arrange
        var client = new ServiceClient
        {
            ClientId = "test-client",
            Name = "Test",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        // Act
        client.LastUsedAt = DateTime.UtcNow;

        // Assert
        client.LastUsedAt.Should().NotBeNull();
        client.LastUsedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
