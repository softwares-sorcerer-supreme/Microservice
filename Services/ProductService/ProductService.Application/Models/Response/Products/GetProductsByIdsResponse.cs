using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public record GetProductsByIdsResponse : BaseResponse
{
    public List<ProductDataResponse> Data { get; set; }
}