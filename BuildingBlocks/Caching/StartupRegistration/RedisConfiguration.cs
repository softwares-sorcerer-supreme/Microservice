using Caching.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Extensions;
using StackExchange.Redis;

namespace Caching.StartupRegistration;

public static class RedisConfiguration
{
    public static IServiceCollection AddRedisConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Redis
        var redisOptions = services.GetOptions<RedisOptions>(RedisOptions.OptionName);
        
        services.AddSingleton<IConnectionMultiplexer>
        (
            opt=>
            ConnectionMultiplexer.Connect(redisOptions.Url)
        );
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration =;
            options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
            {
                AbortOnConnectFail = true,
                EndPoints = { redisOptions.Url }
            };
        });
        
        
        
        return services;
    }
}