using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Shared.Constants;
using Shared.Resilience;

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
        });

        if (clientConfig.IsEnableRetry)
        {
            httpClientBuilder.AddResilienceHandler
            (
                ResiliencePipelineConst.HttpRetry,
                (resiliencePipelineBuilder, _) =>
                    resiliencePipelineBuilder
                        .AddRetry(PollyResilienceStrategies.Retry(clientConfig.Retry, logger))
                        .AddTimeout(PollyResilienceStrategies.Timeout(clientConfig.HttpClientTimeout))
            );
        }
        
        if (clientConfig.IsEnableCircuitBreaker)
        {
            httpClientBuilder.AddResilienceHandler
            (
                ResiliencePipelineConst.HttpCircuitBreaker,
                (resiliencePipelineBuilder, _) =>
                    resiliencePipelineBuilder.AddCircuitBreaker(
                        PollyResilienceStrategies.CircuitBreaker(clientConfig.CircuitBreaker, logger))
            );
        }

        return services;
    }
    
    #endregion
}