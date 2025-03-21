using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using ProductService.Domain.Abstraction;
using ProductService.Domain.Abstraction.Repositories;
using ProductService.Persistence.Repositories;
using Shared.Interceptors;

namespace ProductService.Persistence.StartupRegistration;

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
        services.AddScoped<IReadOnlyRepository, ReadOnlyRepository>();

        return services;
    }
}