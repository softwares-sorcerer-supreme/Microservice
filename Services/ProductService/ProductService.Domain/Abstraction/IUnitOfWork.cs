using ProductService.Domain.Abstraction.Repositories;

namespace ProductService.Domain.Abstraction;

public interface IUnitOfWork : IAsyncDisposable
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    IProductRepository Product { get; set; }
}