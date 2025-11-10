using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechFood.Application;
using TechFood.Domain.Repositories;
using TechFood.Infra.Persistence.Contexts;
using TechFood.Infra.Persistence.Repositories;
using TechFood.Shared.Infra.Extensions;

namespace TechFood.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services)
    {
        services.AddSharedInfra<AuthContext>(new InfraOptions
        {
            DbContext = (serviceProvider, dbOptions) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                dbOptions.UseSqlServer(config.GetConnectionString("DataBaseConection"));
            },
            ApplicationAssembly = typeof(DependecyInjection).Assembly
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IServiceClientRepository, ServiceClientRepository>();

        //MediatR
        services.AddMediatR(typeof(DependecyInjection));

        return services;
    }
}
