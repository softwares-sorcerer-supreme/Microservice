using Shared.Models.Dtos;
using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public class GetProductsResponse : ApiResponse<GetProductsData>;

public record GetProductsData
{
    public PagingDto Paging { get; set; } = new();
    public List<ProductDataResponse> Data { get; set; } = [];
}