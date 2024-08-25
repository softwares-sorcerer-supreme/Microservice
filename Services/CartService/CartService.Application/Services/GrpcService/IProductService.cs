using Microsoft.Extensions.Logging;
using ProductService.Application.Grpc.Protos;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace CartService.Application.Services.GrpcService;

public interface IProductService
{
    Task<GetProductsByIdsResponse> GetProductsByIds(List<Guid> ids);
    Task<ProductModelResponse> UpdateProductQuantity(Guid id, int quantity);
    Task<ProductModelResponse> GetProductsById(Guid id);
}