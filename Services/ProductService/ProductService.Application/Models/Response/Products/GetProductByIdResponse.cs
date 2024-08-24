using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public class GetProductByIdResponse : ErrorResponse
{
    public GetProductData Data { get; set; }
}