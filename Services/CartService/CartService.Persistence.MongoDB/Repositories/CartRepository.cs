using CartService.Domain.Abstraction.Repositories;
using CartService.Domain.Entities;

namespace CartService.Persistence.MongoDB.Repositories;

public class CartRepository(ApplicationMongoDbContext context)
    : RepositoryBase<Cart>(context), ICartRepository;