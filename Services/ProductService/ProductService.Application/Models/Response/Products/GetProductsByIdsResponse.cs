using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public class GetProductsByIdsResponse : BaseResponse
{
    public List<ProductDataResponse> Data { get; set; }
}