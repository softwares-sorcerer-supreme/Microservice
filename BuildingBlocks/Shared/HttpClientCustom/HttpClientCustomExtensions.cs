using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using Shared.HttpClientCustom.Resilience;

namespace Shared.HttpClientCustom;

public static class HttpClientCustomExtensions
{
    #region Public Methods

    public static IServiceCollection AddResilienceHttpClientCustom<T>(this IServiceCollection services, T clientConfig) where T : ResilienceConfig
    {
        var clientName = $"{typeof(T).Name}{DateTime.UtcNow.Ticks}";
        var logger = services.BuildServiceProvider().GetService<ILogger<IHttpClientCustom<T>>>();

        var httpClientBuilder = services.AddHttpClient<IHttpClientCustom<T>, HttpClientCustom<T>>(clientName, c =>
            {
                c.BaseAddress = new Uri(clientConfig.Url);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.Timeout = TimeSpan.FromMinutes(clientConfig.HttpClientTimeout);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(clientConfig.HttpClientTimeout * 2));

        if (clientConfig.IsEnableRetry)
        {
            httpClientBuilder.AddResilienceHandler
            (
                ResiliencePipelineConst.HttpRetry,
                (_, _) => PollyResilienceStrategy.Retry(clientConfig.Retry)
            );
        }
        
        if (clientConfig.IsEnableCircuitBreaker)
        {
            httpClientBuilder.AddResilienceHandler
            (
                ResiliencePipelineConst.HttpCircuitBreaker,
                (_, _) => PollyResilienceStrategy.CircuitBreaker(clientConfig.CircuitBreaker, logger)
            );
        }

        return services;
    }
    
    #endregion
}