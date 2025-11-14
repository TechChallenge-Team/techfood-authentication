using FluentAssertions;
using TechFood.Authentication.Domain.Entities;
using TechFood.Authentication.Domain.ValueObjects;

namespace TechFood.Authentication.Domain.Tests
{
    public class UserTests
    {
        [Fact(DisplayName = "Should create user with valid data")]
        [Trait("Domain", "User")]
        public void User_WithValidData_ShouldCreateSuccessfully()
        {
            // Arrange
            var name = new Name("John Doe");
            var username = "johndoe";
            var role = "Admin";
            var email = new Email("john@example.com");

            // Act
            var user = new User(name, username, role, email);

            // Assert
            user.Should().NotBeNull();
            user.Name.Should().Be(name);
            user.Username.Should().Be(username);
            user.Role.Should().Be(role);
            user.Email.Should().Be(email);
        }

        [Fact(DisplayName = "Should set password hash")]
        [Trait("Domain", "User")]
        public void SetPassword_WithValidHash_ShouldSetPasswordHash()
        {
            // Arrange
            var user = new User(
                new Name("Jane Smith"),
                "janesmith",
                "User",
                new Email("jane@example.com"));

            var passwordHash = "hashed_password_123";

            // Act
            user.SetPassword(passwordHash);

            // Assert
            user.PasswordHash.Should().Be(passwordHash);
        }

        [Fact(DisplayName = "Should set role")]
        [Trait("Domain", "User")]
        public void SetRole_WithValidRole_ShouldSetRole()
        {
            // Arrange
            var user = new User(
                new Name("Bob Johnson"),
                "bobjohnson",
                "User",
                null);

            // Act
            user.SetRole("Manager");

            // Assert
            user.Role.Should().Be("Manager");
        }

        [Fact(DisplayName = "Should throw exception when setting empty password")]
        [Trait("Domain", "User")]
        public void SetPassword_WithEmptyHash_ShouldThrowException()
        {
            // Arrange
            var user = new User(
                new Name("Test User"),
                "testuser",
                "User",
                null);

            // Act & Assert
            Assert.Throws<Shared.Domain.Exceptions.DomainException>(() => user.SetPassword(""));
        }

        [Fact(DisplayName = "Should throw exception when setting empty role")]
        [Trait("Domain", "User")]
        public void SetRole_WithEmptyRole_ShouldThrowException()
        {
            // Arrange
            var user = new User(
                new Name("Test User"),
                "testuser",
                "User",
                null);

            // Act & Assert
            Assert.Throws<Shared.Domain.Exceptions.DomainException>(() => user.SetRole(""));
        }
    }
}
