using System.Collections.Immutable;
using System.Net.Sockets;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace Shared.HttpClientCustom.Resilience;

public static class PollyResilienceStrategy
{
    public static ResiliencePipeline<HttpResponseMessage> CircuitBreaker()
    {
        return new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                FailureRatio =
                    0.5, //default 0.5 // The circuit will break if more than 50% of actions result in handled exceptions,
                SamplingDuration = TimeSpan.FromSeconds(10), //default 10
                MinimumThroughput = 10, //default 10
                BreakDuration = TimeSpan.FromSeconds(30), //default 30
                OnOpened = static args => { return ValueTask.CompletedTask; },
                OnClosed = args => { return ValueTask.CompletedTask; }
            }).Build();
    }

    public static ResiliencePipeline<HttpResponseMessage> Retry()
    {
        ImmutableArray<Type> networkExceptions =
        [
            ..new[]
            {
                typeof(SocketException),
                typeof(HttpRequestException),
            }
        ];

        ImmutableArray<Type> strategyExceptions =
        [
            ..new[]
            {
                typeof(TimeoutRejectedException),
                typeof(BrokenCircuitException),
            }
        ];

        ImmutableArray<Type> retryableExceptions =
        [
            ..networkExceptions
                .Union(strategyExceptions)
        ];

        return new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 3, //default 3
                DelayGenerator = static args =>
                {
                    var delay = args.AttemptNumber switch
                    {
                        0 => TimeSpan.FromMilliseconds(500),
                        1 => TimeSpan.FromSeconds(2),
                        _ => TimeSpan.FromSeconds(5)
                    };
                    return new ValueTask<TimeSpan?>(delay);
                },
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = ex => new ValueTask<bool>(retryableExceptions.Contains(ex.GetType())),
            }).Build();
    }
}