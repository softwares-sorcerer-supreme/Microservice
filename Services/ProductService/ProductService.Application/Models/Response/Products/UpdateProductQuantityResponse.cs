using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public record UpdateProductQuantityResponse : BaseResponse
{
    public ProductDataResponse Data { get; set; }
}