using Caching.Options;
using Caching.Services;
using Microsoft.Extensions.DependencyInjection;
using Shared.Extensions;
using StackExchange.Redis;

namespace Caching.StartupRegistration;

public static class RedisConfiguration
{
    public static IServiceCollection AddRedisConfiguration(this IServiceCollection services)
    {
        // Register Redis
        var redisOptions = services.GetOptions<RedisOptions>(RedisOptions.OptionName);
        var redisUrl = $"{redisOptions.Host}:{redisOptions.Port}";
        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { redisUrl },
            AbortOnConnectFail = false,
            Ssl = redisOptions.IsSSL,
            Password = redisOptions.Password
        };
        
        services.AddSingleton<IConnectionMultiplexer>
        (
            _ => ConnectionMultiplexer.Connect(configurationOptions)
        );

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisUrl;
            options.ConfigurationOptions = configurationOptions;
        });

        services.AddScoped<IRedisService, RedisService>();

        return services;
    }
}