using CartService.Domain.Abstraction.Repositories;
using CartService.Domain.Entities;

namespace CartService.Persistence.MongoDB.Repositories;

public class CartItemRepository(ApplicationMongoDbContext context)
    : RepositoryBase<CartItem>(context), ICartItemRepository;