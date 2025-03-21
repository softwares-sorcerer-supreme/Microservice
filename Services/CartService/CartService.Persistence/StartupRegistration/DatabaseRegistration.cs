using CartService.Domain.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interceptors;

namespace CartService.Persistence.StartupRegistration;

public static class DatabaseRegistration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        
        services.AddDbContext<ApplicationDbContext>(
            (sp, options) => options
                .UseNpgsql(connectionString)
                .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>()));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}