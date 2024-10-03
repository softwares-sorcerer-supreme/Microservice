using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Shared.HttpClientCustom;

public static class HttpClientCustomExtensions
{

    #region Public Methods

    public static IServiceCollection AddHttpClientCustom<T>(this IServiceCollection services, T clientConfig) where T : ServiceConfig
    {
        var clientName = $"{typeof(T).Name}{DateTime.UtcNow.Ticks}" ;
        var logger =  services.BuildServiceProvider().GetService<ILogger<IHttpClientCustom<T>>>();

        services.AddHttpClient<IHttpClientCustom<T>, HttpClientCustom<T>>(clientName, c =>
            {
                c.BaseAddress = new Uri(clientConfig.Url);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.Timeout = TimeSpan.FromMinutes(clientConfig.HttpClientTimeout);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(clientConfig.HttpClientTimeout * 2));

        return services;
    }
    
    #endregion
}