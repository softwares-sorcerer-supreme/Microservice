using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Shared.StartupRegistration;

public static class ResilienceRegistration
{
    public static IServiceCollection AddResilienceConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddResiliencePipeline("retry", builder =>
        {
            builder.AddRetry(new RetryStrategyOptions
            {
                Delay = TimeSpan.FromSeconds(1),
                MaxRetryAttempts = 2,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true
            });
        });
        
        services.AddResiliencePipeline(
            "db-pipeline",
            pipelineBuilder =>
            {
                pipelineBuilder.AddRetry(
                        new RetryStrategyOptions
                        {
                            ShouldHandle = new PredicateBuilder().Handle<DbException>(),
                            Delay = TimeSpan.FromSeconds( 1 ),
                            MaxRetryAttempts = 3,
                            BackoffType = DelayBackoffType.Exponential
                        } )
                    .ConfigureTelemetry(
                        LoggerFactory.Create( loggingBuilder => loggingBuilder.AddConsole() ) );
            } );
        
        return services;
    }
}