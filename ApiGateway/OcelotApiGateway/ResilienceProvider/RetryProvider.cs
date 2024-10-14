using Ocelot.Configuration;
using Ocelot.Provider.Polly.Interfaces;
using Polly;
using Polly.Retry;

namespace ApiGateway.ResilienceProvider;

public class RetryProvider : IPollyQoSResiliencePipelineProvider<HttpResponseMessage>
{
    public ResiliencePipeline<HttpResponseMessage> GetResiliencePipeline(DownstreamRoute route)
    {
        return new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = args => new ValueTask<bool>(args.Outcome.Exception != null),
                Delay = TimeSpan.FromSeconds(1),
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Constant
            })
            .Build();
    }
}