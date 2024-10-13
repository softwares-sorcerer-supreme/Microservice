using CartService.Domain.Abstraction.Repositories;

namespace CartService.Domain.Abstraction;

public interface IUnitOfWorkMongoDb : IAsyncDisposable
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    ICartRepository Cart { get; set; }
    ICartItemRepository CartItem { get; set; }
}