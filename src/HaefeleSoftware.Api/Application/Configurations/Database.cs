using System.Reflection;
using HaefeleSoftware.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HaefeleSoftware.Api.Application.Configurations;

public static class Database
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString(DatabaseSettings.ConnectionString),
                builder => builder.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)
            );
        });

        services.AddScoped<DatabaseContext>();
    }
}