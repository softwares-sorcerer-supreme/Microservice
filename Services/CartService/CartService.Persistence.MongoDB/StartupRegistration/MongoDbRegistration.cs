using CartService.Domain.Abstraction.Repositories.MongoDb;
using CartService.Domain.Entities;
using CartService.Persistence.MongoDB.Options;
using CartService.Persistence.MongoDB.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
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

        services.AddSingleton(database);
        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        services.AddScoped<ICartItemMongoRepository, CartItemRepository>();
        services.AddScoped<ICartMongoRepository, CartRepository>();

        ClassMappingConfiguration(database);

        return services;
    }

    private static void ClassMappingConfiguration(IMongoDatabase database)
    {
        BsonClassMap.RegisterClassMap<Cart>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id)
                .SetIdGenerator(new GuidGenerator())
                .SetSerializer(new GuidSerializer(BsonType.Binary));

            cm.SetIgnoreExtraElements(true);
        });
        
        
        BsonClassMap.RegisterClassMap<CartItem>(cm =>
        {
            cm.AutoMap();
            cm.MapMember(c => c.CartId)
                .SetSerializer(new GuidSerializer(BsonType.String));
            cm.MapMember(c => c.ProductId)
                .SetSerializer(new GuidSerializer(BsonType.String));

            cm.SetIgnoreExtraElements(true);
        });

        // Create a unique index on CartId and ProductId
        var cartItemCollection = database.GetCollection<CartItem>(nameof(CartItem));
        var indexKeysDefinition = Builders<CartItem>.IndexKeys
            .Ascending(c => c.CartId)
            .Ascending(c => c.ProductId);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<CartItem>(indexKeysDefinition, indexOptions);
        cartItemCollection.Indexes.CreateOne(indexModel);
    }
    
}