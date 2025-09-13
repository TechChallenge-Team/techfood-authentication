using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechFood.Application;
using TechFood.Domain.Repositories;
using TechFood.Infra.Persistence.Contexts;
using TechFood.Infra.Persistence.Repositories;

namespace TechFood.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services)
    {
        //Context
        services.AddScoped<TechFoodContext>();
        services.AddDbContext<TechFoodContext>((serviceProvider, options) =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            options.UseSqlServer(config.GetConnectionString("DataBaseConection"));
        });

        services.AddScoped<IUserRepository, UserRepository>();

        //MediatR
        services.AddMediatR(typeof(DependecyInjection));

        return services;
    }
}
