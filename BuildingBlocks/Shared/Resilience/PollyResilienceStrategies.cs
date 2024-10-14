using System.Net;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Shared.HttpClientCustom;

namespace Shared.Resilience;

public static class PollyResilienceStrategies
{
    public static HttpCircuitBreakerStrategyOptions CircuitBreaker(CircuitBreakerOptions options, ILogger logger)
    {
        return new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio =
                options.FailureRatio, //default 0.5 // The circuit will break if more than 50% of actions result in handled exceptions,
            SamplingDuration = TimeSpan.FromSeconds(options.SamplingDurationInSeconds), //default 10
            MinimumThroughput = options.MinimumThroughput, //default 10
            BreakDuration = TimeSpan.FromSeconds(options.BreakDurationInSeconds), //default 30
            OnOpened = args =>
            {
                var message =
                    $"CircuitBreaker => Message = Circuit open for duration {args.BreakDuration.TotalMinutes} minute(s) ({options.BreakDurationInSeconds}s, Time: {DateTime.Now.ToLongTimeString()}";
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
                logger.LogWarning($"CircuitBreaker closed. Time: {DateTime.Now.ToLongTimeString()}");
                return ValueTask.CompletedTask;
            },
            OnHalfOpened = _ =>
            {
                logger.LogWarning($"CircuitBreaker half-opened. Time: {DateTime.Now.ToLongTimeString()}");
                return ValueTask.CompletedTask;
            }
        };
    }

    public static RetryStrategyOptions<HttpResponseMessage> Retry(RetryOptions options, ILogger logger)
    {
        return new HttpRetryStrategyOptions
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
                { Exception: TimeoutException } => PredicateResult.True(),
                { Exception: TaskCanceledException } => PredicateResult.True(),
                {
                    Result.StatusCode: HttpStatusCode.TooManyRequests
                    or HttpStatusCode.InternalServerError
                    or HttpStatusCode.BadGateway
                    or HttpStatusCode.GatewayTimeout
                    or HttpStatusCode.ServiceUnavailable
                } => PredicateResult.True(),
            
                _ => PredicateResult.False()
            },
            OnRetry = args =>
            {
                logger.LogWarning($"Screenshot failed with exception {args.Outcome.Exception?.Message}. " +
                                   $"Waiting {args.Duration} before next retry. Retry attempt {args.AttemptNumber}");
                return ValueTask.CompletedTask;
            }
        };
    }
    
    public static TimeoutStrategyOptions Timeout(int timeoutInSeconds)
    {
        return new TimeoutStrategyOptions
        {
            Timeout = TimeSpan.FromSeconds(timeoutInSeconds),
        };
    }
    
}