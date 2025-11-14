using Bogus;
using TechFood.Authentication.Domain.Entities;
using TechFood.Authentication.Domain.ValueObjects;

namespace TechFood.Authentication.Domain.Tests.Fixtures
{
    public class UserFixture
    {
        private readonly Faker _faker;

        public UserFixture()
        {
            _faker = new Faker("pt_BR");
        }

        public User CreateValidUser(string? username = null, string? role = null)
        {
            var firstName = _faker.Name.FirstName();
            var lastName = _faker.Name.LastName();
            var fullName = $"{firstName} {lastName}";
            var email = _faker.Internet.Email(firstName, lastName);
            
            return new User(
                new Name(fullName),
                username ?? _faker.Internet.UserName(firstName, lastName),
                role ?? "User",
                new Email(email));
        }
    }
}
