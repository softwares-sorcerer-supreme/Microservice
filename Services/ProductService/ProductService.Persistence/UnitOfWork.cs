using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Abstraction;
using ProductService.Domain.Abstraction.Repositories;
using ProductService.Persistence.Repositories;

namespace ProductService.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    public IProductRepository Product { get; set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Product = new ProductRepository(context);
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