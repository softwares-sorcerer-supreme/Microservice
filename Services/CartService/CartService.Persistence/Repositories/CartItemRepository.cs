using CartService.Domain.Abstraction.Repositories;
using CartService.Domain.Entities;

namespace CartService.Persistence.Repositories;

public class CartItemRepository(ApplicationDbContext context)
    : RepositoryBase<CartItem, Guid>(context), ICartItemRepository;