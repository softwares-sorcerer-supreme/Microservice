using AuthService.Application.Abstraction.Interfaces;
using AuthService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.StartupRegistration;

public static class DependencyInjectionRegistration
{
    public static IServiceCollection AddDIConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}