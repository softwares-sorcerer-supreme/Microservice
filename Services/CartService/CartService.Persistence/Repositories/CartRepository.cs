using CartService.Domain.Abstraction.Repositories;
using CartService.Domain.Entities;

namespace CartService.Persistence.Repositories;

public class CartRepository(ApplicationDbContext context)
    : RepositoryBase<Cart, Guid>(context), ICartRepository;