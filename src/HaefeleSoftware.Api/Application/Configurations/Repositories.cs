using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Infrastructure.Repositories;

namespace HaefeleSoftware.Api.Application.Configurations;

public static class Repositories
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }
}