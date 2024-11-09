using CartService.Domain.Abstraction;
using CartService.Domain.Abstraction.Repositories;
using CartService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CartService.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    public ICartRepository Cart { get; set; }
    public ICartItemRepository CartItem { get; set; }

    public UnitOfWork(ApplicationDbContext context)
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