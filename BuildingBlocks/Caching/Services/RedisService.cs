using Caching.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Caching.Services;

public class RedisService : IRedisService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IDatabase Database;

    private const string ClearCacheLuaScript =
    "for _,k in ipairs(redis.call('KEYS', ARGV[1])) do\n" +
    "    redis.call('DEL', k)\n" +
    "end";

    private const int RedisDefaultSlidingExpirationInSecond = 3600;
 
    public RedisService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
    {
        Database = connectionMultiplexer.GetDatabase();
        _distributedCache = distributedCache;
    }

    private ConnectionMultiplexer ConnectionMultiplexer => Database.Multiplexer as ConnectionMultiplexer;

    public T? Get<T>(string key)
    {
        var value = _distributedCache.GetString(key);

        if (!string.IsNullOrWhiteSpace(value))
        {
            return CacheHelper.Deserialize<T>(value);
        }
        return default;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _distributedCache.GetStringAsync(key);

        return !string.IsNullOrWhiteSpace(value) ? CacheHelper.Deserialize<T>(value) : default;
    }

    public void Set<T>(string key, T value)
    {
        _distributedCache.SetString(key, CacheHelper.Serialize(value));
    }

    public void Set<T>(string key, T value, DistributedCacheEntryOptions distributedCacheEntryOptions)
    {
        _distributedCache.SetString(key, CacheHelper.Serialize(value), distributedCacheEntryOptions);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        await _distributedCache.SetStringAsync(key, CacheHelper.Serialize(value));
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions distributedCacheEntryOptions)
    {
        await _distributedCache.SetStringAsync(key, CacheHelper.Serialize(value), distributedCacheEntryOptions);
    }

    public T GetOrSet<T>(string key, Func<T> func)
    {
        return GetOrSet<T>(key, func, TimeSpan.FromSeconds(RedisDefaultSlidingExpirationInSecond));
    }

    public T GetOrSet<T>(string key, Func<T> func, TimeSpan expiration)
    {
        return GetOrSet<T>(key, func, new DistributedCacheEntryOptions() { SlidingExpiration = expiration });
    }

    public T GetOrSet<T>(string key, Func<T> func, DistributedCacheEntryOptions distributedCacheEntryOptions)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be null, empty, or only whitespace.");
        }

        var valueAsString = _distributedCache.GetString(key);

        if (!string.IsNullOrEmpty(valueAsString))
        {
            return CacheHelper.Deserialize<T>(valueAsString);
        }

        var value = func();

        if (value != null)
        {
            _distributedCache.SetString(key, CacheHelper.Serialize(value), distributedCacheEntryOptions);
        }

        return value;
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }

    public async Task RemoveAsync(string[] keys)
    {
        if (keys == null)
        {
            throw new ArgumentNullException(nameof(keys));
        }

        if (!keys.Any())
        {
            return;
        }

        var keysArray = keys.Select(x => (RedisKey)(/*_redisOptions.Prefix + */x))
            .ToArray();

        await Database.KeyDeleteAsync(keysArray);
    }

    public async Task RefreshAsync(string key)
    {
        await _distributedCache.RefreshAsync(key);
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func)
    {
        return await GetOrSetAsync(key, func, TimeSpan.FromSeconds(RedisDefaultSlidingExpirationInSecond));
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, TimeSpan expiration)
    {
        return await GetOrSetAsync(key, func, new DistributedCacheEntryOptions() { SlidingExpiration = expiration });
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, DistributedCacheEntryOptions distributedCacheEntryOptions)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be null, empty, or only whitespace.");
        }

        var valueAsString = await _distributedCache.GetStringAsync(key);

        if (!string.IsNullOrEmpty(valueAsString))
        {
            return CacheHelper.Deserialize<T>(valueAsString);
        }

        var value = await func();

        if (value != null)
        {
            await _distributedCache.SetStringAsync(key, CacheHelper.Serialize(value), distributedCacheEntryOptions);
        }

        return value;
    }

    public async Task ResetAsync()
    {
        await Database.ScriptEvaluateAsync(
            ClearCacheLuaScript,
            values: new RedisValue[]
            {
                    /*_redisOptions.Prefix +*/ "*"
            });
     }

    //public async Task<IEnumerable<string>> GetKeysAsync()
    //{
    //    return await GetKeysAsync("*");
    //}

    //public async Task<IEnumerable<string>> GetKeysAsync(string pattern)
    //{
    //    var result = await Database.ScriptEvaluateAsync(
    //           GetKeysLuaScript,
    //           values: new RedisValue[]
    //           {
    //                _redisOptions.Prefix + pattern
    //           });

    //    return ((RedisResult[])result).Select(x => x.ToString().Substring(_redisOptions.Prefix.Length))
    //        .ToArray();
    //}

    public void Remove(string key)
    {
        _distributedCache.Remove(key);
    }

    public void Subscribe(string channelName, Action<RedisChannel, RedisValue> handler)
    {
        Database.Multiplexer
            .GetSubscriber()
            .Subscribe(channelName, handler);
    }

    public ISubscriber GetSubscriber()
    {
        return Database.Multiplexer.GetSubscriber();
    }

    public async Task<long> PublishAsync<T>(string channelName, T message)
    {
        return await Database
            .PublishAsync(channelName, CacheHelper.Serialize(message), CommandFlags.FireAndForget);
    }

    public async Task<long> StringIncrementAsync(string key, long incrementValue = 1)
    {
        return await Database
            .StringIncrementAsync(key, incrementValue, CommandFlags.FireAndForget);
    }

    public async Task<long> StringDecrementAsync(string key, long decrementValue = 1)
    {
        return await Database
            .StringDecrementAsync(key, decrementValue, CommandFlags.FireAndForget);
    }

    public async Task<long> HashIncrementAsync(string key, string hashField, long incrementValue = 1)
    {
        return await Database
            .HashIncrementAsync(key, hashField.ToLower(), incrementValue, CommandFlags.FireAndForget);
    }

    public async Task<long> HashDecrementAsync(string key, string hashField, long decrementValue = 1)
    {
        return await Database
            .HashDecrementAsync(key, hashField.ToLower(), decrementValue, CommandFlags.None);
    }

    public async Task<bool> HashDeleteAsync(string key, string hashField)
    {
        return await Database
            .HashDeleteAsync(key, hashField.ToLower());
    }

    public async Task<long> HashDeleteAsync(string key, string[] hashFields)
    {
        return await Database
            .HashDeleteAsync(key, hashFields.Select(x => (RedisValue)x.ToLower())
            .ToArray());
    }

    public async Task<bool> HashExistsAsync(string key, string hashField)
    {
        return await Database.HashExistsAsync(key, hashField.ToLower());
    }

    public async Task<bool> KeyDeleteAsync(string key)
    {
        return await Database.KeyDeleteAsync(key);
    }

    public async Task<T> HashGetAsync<T>(string key, string hashField)
    {
        var redisValue = await Database.HashGetAsync(key, hashField.ToLower());

        return redisValue.HasValue
            ? CacheHelper.Deserialize<T>(redisValue.ToString())
            : default;
    }

    public async Task<IEnumerable<T>> HashGetAsync<T>(string key, string[] hashFields)
    {
        var redisValues = await Database
            .HashGetAsync(key, hashFields.Select(x => (RedisValue)x.ToLower())
                .ToArray());

        return redisValues?.Select(x => CacheHelper.Deserialize<T>(x))
            .AsEnumerable();
    }

    public async Task<Dictionary<string, T>> HashGetAsyncV2<T>(string key, string[] hashFields)
    {
        var redisValues = await Database
             .HashGetAsync(key, hashFields.Select(x => (RedisValue)x.ToLower())
             .ToArray());

        var result = new Dictionary<string, T>();

        for (var i = 0; i < redisValues?.Length; i++)
            result.Add(hashFields[i], CacheHelper.Deserialize<T>(redisValues[i]));

        return result;
    }

    // pipeline
    public async Task<Dictionary<string, T>[]> HashGetAllAsync<T>(string[] keys)
    {
        var batch = Database.CreateBatch();

        var tasks = keys.Select(x => batch.HashGetAllAsync(x)).ToArray();

        batch.Execute();
        Database.WaitAll(tasks);

        var results = new Dictionary<string, T>[keys.Length];
        for (int i = 0; i < keys.Length; i++)
        {
            var entry = await tasks[i];
            results[i] = entry.ToDictionary(
                x => x.Name.ToString(),
                x => CacheHelper.Deserialize<T>(x.Value),
                StringComparer.Ordinal);
        }

        return results;
    }

    public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string key)
    {
        var entries = await Database.HashGetAllAsync(key);
        return entries?.ToDictionary(
            x => x.Name.ToString(),
            x => CacheHelper.Deserialize<T>(x.Value),
            StringComparer.Ordinal);
    }

    public Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, TimeSpan? expiration = null)
    {
        var entries = values.Select(entry => new HashEntry(entry.Key, CacheHelper.Serialize(entry.Value)));
        var result = Database.HashSetAsync(hashKey, entries.ToArray());
        if (expiration.HasValue)
        {
            Database.KeyExpireAsync(hashKey, expiration.Value);
        }
        return result;
    }

    public async Task<IEnumerable<T>> HashValuesAsync<T>(string key)
    {
        var entries = await Database.HashValuesAsync(key);
        return entries?.Select(x => CacheHelper.Deserialize<T>(x)).AsEnumerable();
    }

    public async Task<T> HashGetOrSetAsync<T>(string key, string hashField, Func<Task<T>> func, TimeSpan? expiry)
    {
        string redisValue = await Database.HashGetAsync(key, hashField.ToLower());

        if (!string.IsNullOrEmpty(redisValue))
        {
            return CacheHelper.Deserialize<T>(redisValue);
        }

        var value = await func();

        if (value != null)
        {
            await HashSetAsync(key, hashField.ToLower(), value, expiry);
        }

        return value;
    }

    public async Task<Dictionary<string, T>> HashGetAllOrSetAsync<T>(string key, Func<Task<Dictionary<string, T>>> func, TimeSpan? expiration = null)
    {
        Dictionary<string, T> entries = await HashGetAllAsync<T>(key);
        if (entries != null)
        {
            return entries;
        }

        entries = await func();
        if (entries != null)
        {
            await HashSetAsync<T>(key, entries, expiration);
        }
        else
        {
            entries = new Dictionary<string, T>();
        }

        return entries;
    }

    public async Task<Dictionary<string, T>> HashSetAndGetAsync<T>(string key, Func<Task<Dictionary<string, T>>> func)
    {
        Dictionary<string, T> entries = await func();
        await HashSetAsync(key, entries);

        return entries;
    }

    public async Task HashSetLongAsync(string key, string hashField, long value)
    {
        await Database.HashSetAsync(key, hashField.ToLower(), value);
    }

    public async Task HashSetAsync(string key, string hashField, object value, TimeSpan? expiry)
    {
        await Database.HashSetAsync(key, hashField.ToLower(), CacheHelper.Serialize(value));

        if (expiry.HasValue)
        {
            await Database.HashFieldExpireAsync
            (
                new RedisKey(key),
                [hashField],
                expiry.Value
            );
        }
    }

    public async Task<IEnumerable<string>> HashKeysAsync(string key)
    {
        var redisValues = await Database.HashKeysAsync(key);
        if (redisValues == null)
        {
            return new List<string>();
        }
        return redisValues?.Select(x => (string)x)
            .AsEnumerable();
    }

    /// <inheritdoc/>
    public Task<long> HashIncrementByAsync(string hashKey, string key, long value)
    {
        return Database.HashIncrementAsync(hashKey, key, value);
    }

    public Task<double> HashIncrementByAsync(string hashKey, string key, double value)
    {
        return Database.HashIncrementAsync(hashKey, key, value);
    }

    public Task<long> HashLengthAsync(string hashKey)
    {
        return Database.HashLengthAsync(hashKey);
    }

    public async Task<IEnumerable<string>> HashKeysAsync(string key, string hashFieldPattern, int count = int.MaxValue)
    {
        var cursor = 0;

        // HSCAN FrontOffice_UserByUsername 0 MATCH agent* COUNT 100
        var response = await Database.ScriptEvaluateAsync($"return redis.call('HSCAN', '{key}', {cursor}, 'MATCH', '{hashFieldPattern}', 'COUNT', {count})");

        {
            var result = (RedisResult[])response;

            if (result != null && result.Length > 1)
            {
                var keyValues = (RedisResult[])result[1];
                var hashFieldNames = new List<string>();
                if (keyValues != null)
                {
                    hashFieldNames.AddRange(keyValues
                        .Where((t, i) => i % 2 == 0)
                        .Select(t => t.ToString()));
                }

                return hashFieldNames.ToArray();
            }
        }

        return [];
    }

    /// <summary>
    /// Lock on a resource, giving up immediately if the lock is not available
    /// </summary>
    public async Task<RedLockInstanceSummary> AcquireLockAsync(string resource, TimeSpan expiryTime, Func<Task> func, CancellationToken cancellationToken = default)
    {
        using var redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { ConnectionMultiplexer as ConnectionMultiplexer });

        // the lock is automatically released at the end of the using block
        await using var redLock = await redLockFactory
            .CreateLockAsync(resource, expiryTime);

        if (redLock.IsAcquired)
        {
            await func();
        }

        return redLock.InstanceSummary;
    }

    /// <summary>
    /// Lock on a resource, blocking and retrying every retry seconds until the lock is available, or wait seconds have passed
    /// </summary>
    /// <param name="resource">The thing which is locking on</param>
    /// <param name="expiryTime">Expire after</param>
    /// <param name="waitTime">Wait time before acquire</param>
    /// <param name="retryTime">Retry interval until the lock is available</param>
    /// <param name="func">Callback function</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<RedLockInstanceSummary> AcquireLockAsync(string resource, TimeSpan expiryTime, TimeSpan waitTime, TimeSpan retryTime, Func<Task> func, CancellationToken cancellationToken = default)
    {
        using var redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { ConnectionMultiplexer as ConnectionMultiplexer });

        // the lock is automatically released at the end of the using block
        await using var redLock = await redLockFactory
            .CreateLockAsync(resource, expiryTime, waitTime, retryTime, cancellationToken);

        if (redLock.IsAcquired)
        {
            await func();
        }

        return redLock.InstanceSummary;
    }

    /// <summary>
    /// Lock on a resource, blocking and retrying every retry seconds until the lock is available, or wait seconds have passed
    /// </summary>
    /// <param name="resource">The thing which is locking on</param>
    /// <param name="expiryTime">Expire after</param>
    /// <param name="waitTime">Wait time before acquire</param>
    /// <param name="retryTime">Retry interval until the lock is available</param>
    /// <param name="func">Callback function</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T> AcquireLockAsync<T>(string resource, TimeSpan expiryTime, TimeSpan waitTime, TimeSpan retryTime, Func<Task<T>> func, CancellationToken cancellationToken = default)
    {
        using var redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { ConnectionMultiplexer as ConnectionMultiplexer });

        // the lock is automatically released at the end of the using block
        await using var redLock = await redLockFactory
            .CreateLockAsync(resource, expiryTime, waitTime, retryTime, cancellationToken);

        if (redLock.IsAcquired)
        {
            return await func();
        }

        return default;
    }

    /// <summary>
    /// Lock on a resource, giving up immediately if the lock is not available
    /// </summary>
    /// <param name="resource">The thing which is locking on</param>
    /// <param name="expiryTime">Expire after</param>
    /// <param name="func">Callback function</param>
    /// <returns></returns>
    public async Task<T> AcquireLockAsync<T>(string resource, TimeSpan expiryTime, Func<Task<T>> func)
    {
        using var redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { ConnectionMultiplexer as ConnectionMultiplexer });

        // the lock is automatically released at the end of the using block
        await using var redLock = await redLockFactory
            .CreateLockAsync(resource, expiryTime);

        if (redLock.IsAcquired)
        {
            return await func();
        }

        return default;
    }

    #region Lists and Sets

    public Task<long> ListLeftPushAsync<T>(string key, T item, When when = When.Always)
        where T : class
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("key cannot be empty.", nameof(key));

        if (item == null)
            throw new ArgumentNullException(nameof(item), "item cannot be null.");

        var serializedItem = CacheHelper.Serialize(item);

        return Database.ListLeftPushAsync(key, serializedItem, when);
    }

    /// <inheritdoc/>
    public Task<long> ListLeftPushAsync<T>(string key, T[] items)
        where T : class
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("key cannot be empty.", nameof(key));

        if (items == null)
            throw new ArgumentNullException(nameof(items), "item cannot be null.");

        var serializedItems = items.Select(x => (RedisValue)CacheHelper.Serialize(x)).ToArray();

        return Database.ListLeftPushAsync(key, serializedItems);
    }

    public async Task<T> ListRightPopAsync<T>(string key)
        where T : class
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("key cannot be empty.", nameof(key));

        var item = await Database.ListRightPopAsync(key).ConfigureAwait(false);

        return item == RedisValue.Null
                                ? null
                                : CacheHelper.Deserialize<T>(item);
    }

    /// <inheritdoc/>
    public Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
    {
        var entryBytes = CacheHelper.Serialize(value);
        return Database.SortedSetAddAsync(key, entryBytes, score);
    }

    /// <inheritdoc/>
    public Task<bool> SortedSetRemoveAsync<T>(string key, T value)
    {
        var entryBytes = CacheHelper.Serialize(value);
        return Database.SortedSetRemoveAsync(key, entryBytes);
    }

    /// <inheritdoc/>
    public Task<double?> SortedSetScoreAsync<T>(string key, T value)
    {
        var entryBytes = CacheHelper.Serialize(value);
        return Database.SortedSetScoreAsync(key, entryBytes);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> SortedSetRangeByScoreAsync<T>(
        string key,
        double start = double.NegativeInfinity,
        double stop = double.PositiveInfinity,
        Exclude exclude = Exclude.None,
        Order order = Order.Ascending,
        long skip = 0L,
        long take = -1L)
    {
        var result = await Database.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take).ConfigureAwait(false);
        return result.Select(m => m == RedisValue.Null ? default : CacheHelper.Deserialize<T>(m));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        var result = await Database.SortedSetRangeByRankAsync(key, start, stop, order, flags).ConfigureAwait(false);
        return result.Select(m => m == RedisValue.Null ? default : CacheHelper.Deserialize<T>(m));
    }

    /// <inheritdoc/>
    public async Task<T> SortedSetGetLowestScoreAsync<T>(string key)
    {
        var entries = await Database.SortedSetRangeByRankWithScoresAsync(key, 0, 0).ConfigureAwait(false);
        var result = entries.Select(m => m.Element == RedisValue.Null ? default : CacheHelper.Deserialize<T>(m.Element)).FirstOrDefault();
        return result;
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        var length = await Database.SortedSetLengthAsync(key, min, max, exclude, flags).ConfigureAwait(false);
        return length;
    }

    /// <inheritdoc/>
    public async Task<double?> SortedSetScoreAsync<T>(RedisKey key, T member, CommandFlags flags = CommandFlags.None)
    {
        var entryBytes = CacheHelper.Serialize(member);
        return await Database.SortedSetScoreAsync(key, entryBytes, flags).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<double> SortedSetIncrementAsync<T>(string key, T member, long value = 1)
    {
        var entryBytes = CacheHelper.Serialize(member);
        return await Database.SortedSetIncrementAsync(key, entryBytes, value).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<double> SortedSetDecrementAsync<T>(string key, T member, long value = 1)
    {
        var entryBytes = CacheHelper.Serialize(member);
        return await Database.SortedSetDecrementAsync(key, entryBytes, value).ConfigureAwait(false);
    }

    #endregion Lists and Sets

    public async Task KeysDeleteAsync(params string[] keys)
    {
        if (!keys.Any()) return;

        await this.Database.KeyDeleteAsync(keys.Select(x => (RedisKey)x).ToArray());
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        return await Database.KeyExistsAsync(key);
    }

    /// <inheritdoc/>
    public async Task<bool> SortedSetAddWithExpireAsync<T>(string key, T value, double score, TimeSpan? expiry = null)
    {
        var entryBytes = CacheHelper.Serialize(value);
        var isAdded = await Database.SortedSetAddAsync(key, entryBytes, score).ConfigureAwait(false); ;
        return isAdded && (expiry is null || await Database.KeyExpireAsync(key, expiry, CommandFlags.None));
    }

    public async Task<bool> KeyExpireAsync(string key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
    {
        return await Database.KeyExpireAsync(key, expiry, flags);
    }
}