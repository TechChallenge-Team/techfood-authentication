using System;
using System.Threading.Tasks;
using TechFood.Authentication.Domain.Entities;

namespace TechFood.Authentication.Domain.Repositories;

public interface IUserRepository
{
    Task<Guid> AddAsync(User user);

    Task<User?> GetByUsernameOrEmailAsync(string username);
}
