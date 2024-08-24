using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using ProductService.Domain.Abstraction;

namespace ProductService.Persistence.StartupRegistration;

public static class DatabaseRegistration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionStringBuilder.ConnectionString));
    
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}