using CartService.Domain.Abstraction.Repositories.MongoDb;
using CartService.Domain.Entities;
using MongoDB.Driver;

namespace CartService.Persistence.MongoDB.Repositories;

public class CartItemRepository : MongoRepository<CartItem>, ICartItemMongoRepository
{
    public CartItemRepository(IMongoDatabase database) : base(database)
    {
    }
}