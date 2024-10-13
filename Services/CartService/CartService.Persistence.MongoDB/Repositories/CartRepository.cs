using CartService.Domain.Abstraction.Repositories.MongoDb;
using CartService.Domain.Entities;
using MongoDB.Driver;

namespace CartService.Persistence.MongoDB.Repositories;

public class CartRepository : MongoRepository<Cart>, ICartMongoRepository
{
    public CartRepository(IMongoDatabase database) : base(database)
    {
    }
}