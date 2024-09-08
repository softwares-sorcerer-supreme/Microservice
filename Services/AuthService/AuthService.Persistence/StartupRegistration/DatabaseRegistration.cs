using AuthService.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace AuthService.Persistence.StartupRegistration;

public static class DatabaseRegistration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

        return services;
    }
}