using CartService.Domain.Abstraction;
using CartService.Domain.Abstraction.Repositories;
using CartService.Persistence.MongoDB.Options;
using CartService.Persistence.MongoDB.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Shared.Extensions;

namespace CartService.Persistence.MongoDB.StartupRegistration;

public static class MongoDbRegistration
{
    public static IServiceCollection AddMongoDbConfiguration(this IServiceCollection services)
    {
        // MongoDB configuration
        var mongoDbSettings = services.GetOptions<MongoDBOptions>(MongoDBOptions.OptionName);
        var connectionString = $"mongodb://{mongoDbSettings.Username}:{mongoDbSettings.Password}@{mongoDbSettings.HostName}:{mongoDbSettings.Port}/{mongoDbSettings.DatabaseName}";
        var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));

        var mongoClient = new MongoClient(settings);
        var database = mongoClient.GetDatabase(mongoDbSettings.DatabaseName);

        // Can use EF Core with MongoDB
        services.AddDbContext<ApplicationMongoDbContext>(o => o.UseMongoDB(mongoClient, mongoDbSettings.DatabaseName));

        // Register the MongoDB context or direct collections
        services.AddSingleton(database);
        services.AddScoped<IUnitOfWorkMongoDb, UnitOfWorkMongoDb>();
        services.AddScoped<ICartItemRepository, CartItemRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

        return services;
    }
}