namespace Caching;

public interface IInvalidateCacheRequest
{
    string CacheKey { get; }
}