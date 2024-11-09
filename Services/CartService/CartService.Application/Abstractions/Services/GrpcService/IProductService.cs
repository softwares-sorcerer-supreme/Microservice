using ProductService.Application.Grpc.Protos;

namespace CartService.Application.Services.GrpcService;

public interface IProductService
{
    Task<GetProductsByIdsResponse> GetProductsByIds(List<Guid> ids);

    Task<ProductModelResponse> UpdateProductQuantity(Guid id, int quantity);

    Task<ProductModelResponse> GetProductsById(Guid id);
}