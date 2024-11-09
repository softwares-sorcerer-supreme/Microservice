using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Services;
using ProductService.Infrastructure.Services;

namespace ProductService.Infrastructure.StartupRegistration;

public static class ServicesRegistration
{
    public static IServiceCollection AddDIConfiguration(this IServiceCollection services)
    {
        services.AddScoped<ICartClient, CartClient>();

        return services;
    }
}