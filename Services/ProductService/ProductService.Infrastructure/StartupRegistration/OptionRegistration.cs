using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Infrastructure.Options;
using Shared.HttpClientCustom;

namespace ProductService.Infrastructure.StartupRegistration;

public static class OptionRegistration
{
    public static IServiceCollection AddOptionConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GrpcOptions>(configuration.GetSection(GrpcOptions.OptionName));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.OptionName));
        services.Configure<ServiceOptions>(configuration.GetSection(ServiceOptions.OptionName));

        return services;
    }
}