using Caching.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Caching.Behaviors;

public class InvalidateCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<InvalidateCachingBehavior<TRequest, TResponse>> _logger;
    private readonly IRedisService _redisService;

    public InvalidateCachingBehavior
    (
        ILogger<InvalidateCachingBehavior<TRequest, TResponse>> logger,
        IRedisService redisService
    )
    {
        _logger = logger;
        _redisService = redisService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not IInvalidateCacheRequest invalidateCacheRequest)
        {
            // No cache request found, so just continue through the pipeline
            return await next();
        }

        var cacheKey = invalidateCacheRequest.CacheKey;
        var response = await next();

        await _redisService.RemoveAsync(cacheKey);

        _logger.LogDebug("Cache data with cache key: {CacheKey} removed.", cacheKey);

        return response;
    }
}