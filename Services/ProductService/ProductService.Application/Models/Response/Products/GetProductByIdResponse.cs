using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public class GetProductByIdResponse : ErrorResponse
{
    public ProductDataResponse Data { get; set; }
}