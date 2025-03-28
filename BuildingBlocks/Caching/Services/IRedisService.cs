﻿using Microsoft.Extensions.Caching.Distributed;
using RedLockNet;
using StackExchange.Redis;

namespace Caching.Services;

public interface IRedisService
{
    Task<T?> GetAsync<T>(string key);

    Task SetAsync<T>(string key, T value);

    Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions distributedCacheEntryOptions);

    Task RemoveAsync(string key);

    Task RemoveAsync(string[] keys);

    Task ResetAsync();

    Task RefreshAsync(string key);

    void Set<T>(string key, T value, DistributedCacheEntryOptions distributedCacheEntryOptions);

    T GetOrSet<T>(string key, Func<T> func, DistributedCacheEntryOptions distributedCacheEntryOptions);

    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> func, DistributedCacheEntryOptions distributedCacheEntryOptions);

    void Subscribe(string channelName, Action<RedisChannel, RedisValue> handler);

    ISubscriber GetSubscriber();

    Task<long> PublishAsync<T>(string channelName, T message);

    Task<long> StringIncrementAsync(string key, long incrementValue = 1);

    Task<long> StringDecrementAsync(string key, long decrementValue = 1);

    Task<long> HashIncrementAsync(string key, string hashField, long incrementValue = 1);

    Task<long> HashDecrementAsync(string key, string hashField, long decrementValue = 1);

    Task<bool> HashDeleteAsync(string key, string hashField);

    Task<long> HashDeleteAsync(string key, string[] hashFields);

    Task<bool> HashExistsAsync(string key, string hashField);

    Task<bool> KeyDeleteAsync(string key);

    Task<IEnumerable<string>> HashKeysAsync(string key);

    Task<IEnumerable<string>> HashKeysAsync(string key, string hashFieldPattern, int count = int.MaxValue);

    Task<T> HashGetAsync<T>(string key, string hashField);

    Task<IEnumerable<T>> HashValuesAsync<T>(string key);

    Task<IEnumerable<T>> HashGetAsync<T>(string key, string[] hashFields);

    Task<T> HashGetOrSetAsync<T>(string key, string hashField, Func<Task<T>> func, TimeSpan? expiry = null);

    Task HashSetLongAsync(string key, string hashField, long value);

    Task HashSetAsync(string key, string hashField, object value, TimeSpan? expiry = null);

    Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, TimeSpan? expiration = null);

    Task<Dictionary<string, T>> HashGetAsyncV2<T>(string key, string[] hashFields);

    Task<double?> SortedSetScoreAsync<T>(string key, T value);

    Task<bool> SortedSetAddAsync<T>(string key, T value, double score);

    Task<IEnumerable<T>> SortedSetRangeByScoreAsync<T>(string key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);

    Task<bool> SortedSetRemoveAsync<T>(string key, T value);

    Task<T> SortedSetGetLowestScoreAsync<T>(string key);

    Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    Task<double?> SortedSetScoreAsync<T>(RedisKey key, T member, CommandFlags flags = CommandFlags.None);

    Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    Task<double> SortedSetIncrementAsync<T>(string key, T member, long value = 1);

    Task<double> SortedSetDecrementAsync<T>(string key, T member, long value = 1);

    Task<RedLockInstanceSummary> AcquireLockAsync(string resource, TimeSpan expiryTime, Func<Task> func, CancellationToken cancellationToken = default);

    Task<RedLockInstanceSummary> AcquireLockAsync(string resource, TimeSpan expiryTime, TimeSpan waitTime, TimeSpan retryTime, Func<Task> func, CancellationToken cancellationToken = default);

    Task<T> AcquireLockAsync<T>(string resource, TimeSpan expiryTime, TimeSpan waitTime, TimeSpan retryTime, Func<Task<T>> func, CancellationToken cancellationToken = default);

    Task<T> AcquireLockAsync<T>(string resource, TimeSpan expiryTime, Func<Task<T>> func);

    Task<Dictionary<string, T>> HashGetAllAsync<T>(string key);

    Task<Dictionary<string, T>[]> HashGetAllAsync<T>(string[] keys);

    Task<Dictionary<string, T>> HashGetAllOrSetAsync<T>(string key, Func<Task<Dictionary<string, T>>> func, TimeSpan? expiration = null);

    Task<Dictionary<string, T>> HashSetAndGetAsync<T>(string key, Func<Task<Dictionary<string, T>>> func);

    Task KeysDeleteAsync(params string[] keys);

    Task<long> HashIncrementByAsync(string hashKey, string key, long value);

    Task<bool> KeyExistsAsync(string key);

    Task<bool> SortedSetAddWithExpireAsync<T>(string key, T value, double score, TimeSpan? expiry = null);

    Task<bool> KeyExpireAsync(string key, DateTime? expiry, CommandFlags flags = CommandFlags.None);
}