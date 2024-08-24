using CartService.Domain.Abstraction.Repositories;

namespace CartService.Domain.Abstraction;

public interface IUnitOfWork : IAsyncDisposable
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    ICartRepository Cart { get; set; }
    ICartItemRepository CartItem { get; set; }
}