using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public class GetProductsByIdsResponse : ErrorResponse
{
    public List<GetProductData> Data { get; set; }
}