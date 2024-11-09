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

        services.AddSingleton<IConnectionMultiplexer>
        (
            opt =>
             ConnectionMultiplexer.Connect(new ConfigurationOptions
             {
                 EndPoints = { redisUrl },
                 AbortOnConnectFail = false,
                 Ssl = redisOptions.IsSSL,
                 Password = redisOptions.Password
             })
        );

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisUrl;
            options.ConfigurationOptions = new ConfigurationOptions()
            {
                AbortOnConnectFail = true,
                EndPoints = { redisUrl }
            };
        });

        services.AddScoped<IRedisService, RedisService>();

        return services;
    }
}