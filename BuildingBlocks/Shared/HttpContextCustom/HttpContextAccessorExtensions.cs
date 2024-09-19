using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shared.HttpContextCustom;

public static class HttpContextAccessorExtensions
{
    public static IServiceCollection AddCustomHttpContextAccessor(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.TryAddSingleton<ICustomHttpContextAccessor, CustomHttpContextAccessor>();
        return services;
    } 
}