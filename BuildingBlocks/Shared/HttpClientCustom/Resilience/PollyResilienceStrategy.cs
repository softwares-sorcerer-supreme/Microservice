using System.Collections.Immutable;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace Shared.HttpClientCustom.Resilience;

public static class PollyResilienceStrategy
{
    public static void CircuitBreaker(CircuitBreakerOptions options, ILogger logger)
    {
        new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                FailureRatio = options.FailureRatio, //default 0.5 // The circuit will break if more than 50% of actions result in handled exceptions,
                SamplingDuration = TimeSpan.FromSeconds(options.SamplingDurationInSeconds), //default 10
                MinimumThroughput = options.MinimumThroughput, //default 10
                BreakDuration = TimeSpan.FromSeconds(options.BreakDurationInSeconds), //default 30
                OnOpened = args =>
                {
                    var message =
                        $"CircuitBreaker => Message = Circuit open for duration {args.BreakDuration.TotalMinutes} minute(s) ({options.BreakDurationInSeconds}s)";
                    if (args.Outcome.Exception is null)
                    {
                        logger.LogInformation($"{message}");
                        return ValueTask.CompletedTask;
                    }

                    logger.LogWarning($"{message}, Exception = {args.Outcome.Exception.Message}.");
                    return ValueTask.CompletedTask;
                },
                OnClosed = _ =>
                {
                    logger.LogWarning($"Circuit breaker closed.");
                    return ValueTask.CompletedTask;
                },
                OnHalfOpened = _ =>
                {
                    logger.LogWarning($"Circuit breaker half-opened.");
                    return ValueTask.CompletedTask;
                },
            }).Build();
    }

    public static void Retry(RetryOptions options)
    {
        new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = options.MaxRetryAttempts, //default 3
                Delay = TimeSpan.FromSeconds(options.DelayDurationInSeconds),
                // DelayGenerator = static args =>
                // {
                //     var delay = args.AttemptNumber switch
                //     {
                //         0 => TimeSpan.FromMilliseconds(options.SleepDurationInSecondsFirstAttempt),
                //         1 => TimeSpan.FromSeconds(2),
                //         _ => TimeSpan.FromSeconds(5)
                //     };
                //     return new ValueTask<TimeSpan?>(delay);
                // },
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = args => args.Outcome switch
                {
                    { Exception: HttpRequestException } => PredicateResult.True(),
                    { Exception: TimeoutRejectedException } => PredicateResult.True(),
                    {
                        Result.StatusCode: HttpStatusCode.TooManyRequests
                        or HttpStatusCode.BadGateway
                        or HttpStatusCode.GatewayTimeout
                        or HttpStatusCode.ServiceUnavailable
                    } => PredicateResult.True(),
                    
                    _ => PredicateResult.False()
                }
                
            }).Build();
    }
}