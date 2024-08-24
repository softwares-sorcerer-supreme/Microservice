using ProductService.Domain.Entities;

namespace ProductService.Domain.Abstraction.Repositories
{
    public interface IProductRepository : IRepositoryBase<Product, Guid>
    {
    }
}
