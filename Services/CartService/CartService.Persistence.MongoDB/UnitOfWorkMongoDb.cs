using CartService.Domain.Abstraction;
using CartService.Domain.Abstraction.Repositories;
using CartService.Persistence.MongoDB.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CartService.Persistence.MongoDB;

public class UnitOfWorkMongoDb : IUnitOfWorkMongoDb
{
    private readonly DbContext _context;
    public ICartRepository Cart { get; set; }
    public ICartItemRepository CartItem { get; set; }

    public UnitOfWorkMongoDb
    (
        ApplicationMongoDbContext context
    )
    {
        _context = context;
        Cart = new CartRepository(context);
        CartItem = new CartItemRepository(context);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}