using ProductService.Domain.Abstraction.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Persistence.Repositories
{
    public class ProductRepository(ApplicationDbContext context)
        : RepositoryBase<Product, Guid>(context), IProductRepository;
}