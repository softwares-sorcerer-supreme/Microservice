using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public class GetProductByIdResponse : BaseResponse
{
    public ProductDataResponse Data { get; set; }
}