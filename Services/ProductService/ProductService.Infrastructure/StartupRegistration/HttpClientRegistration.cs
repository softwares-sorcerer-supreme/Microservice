using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Extensions;
using Shared.HttpClientCustom;

namespace ProductService.Infrastructure.StartupRegistration;

public static class HttpClientRegistration
{
    public static IServiceCollection AddHttpClientCustom(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceOptions = services.GetOptions<ServiceOptions>(ServiceOptions.OptionName);
        services.AddResilienceHttpClientCustom(serviceOptions.CartService);

        return services;
    }
}