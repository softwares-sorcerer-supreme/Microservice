using CartService.Domain.Entities;

namespace CartService.Domain.Abstraction.Repositories.MongoDb;

public interface ICartMongoRepository : IMongoRepository<Cart>;