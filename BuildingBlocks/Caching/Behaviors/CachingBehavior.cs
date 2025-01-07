using BuildingBlocks.Caching;
using Caching.Services;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Caching.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
    private readonly IRedisService _redisService;
    private const int DefaultCacheExpirationInHours = 1;

    public CachingBehavior
    (
        ILogger<CachingBehavior<TRequest, TResponse>> logger,
        IRedisService redisService
    )
    {
        _logger = logger;
        _redisService = redisService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICacheRequest cacheRequest)
            // No cache request found, so just continue through the pipeline
        {
            return await next();
        }

        var cacheKey = cacheRequest.CacheKey;
        var cachedResponse = await _redisService.GetAsync<TResponse>(cacheKey);
        if (cachedResponse != null)
        {
            _logger.LogDebug("Response retrieved {TRequest} from cache. CacheKey: {CacheKey}",
                typeof(TRequest).FullName, cacheKey);
            return cachedResponse;
        }

        var response = await next();

        var expirationTime = cacheRequest.AbsoluteExpirationRelativeToNow ??
                             DateTime.Now.AddHours(DefaultCacheExpirationInHours);

        await _redisService.SetAsync(cacheKey, response, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime.TimeOfDay
        });

        _logger.LogDebug("Caching response for {TRequest} with cache key: {CacheKey}", typeof(TRequest).FullName,
            cacheKey);

        return response;
    }
}
