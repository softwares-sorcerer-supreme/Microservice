using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public class UpdateProductQuantityResponse : ErrorResponse
{
    public ProductDataResponse Data { get; set; }
}