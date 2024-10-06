using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public class UpdateProductQuantityResponse : BaseResponse
{
    public ProductDataResponse Data { get; set; }
}