using CartService.Domain.Abstraction;
using CartService.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Persistence.StartupRegistration;

public static class DatabaseRegistration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddSingleton<AuditableEntityInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp, builder) => builder
            .UseNpgsql(connectionString)
            .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>()));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}