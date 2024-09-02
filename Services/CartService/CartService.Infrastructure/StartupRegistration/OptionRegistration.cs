using CartService.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Infrastructure.StartupRegistration;

public static class OptionRegistration
{
    public static IServiceCollection AddOptionConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GrpcOptions>(configuration.GetSection(GrpcOptions.OptionName));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.OptionName));
        
        return services;
    }
}