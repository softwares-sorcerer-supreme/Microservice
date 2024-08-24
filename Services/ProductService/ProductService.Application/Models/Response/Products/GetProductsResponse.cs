using ReviewVerse.Shared.Models.Dtos;
using Shared.Models.Response;

namespace ProductService.Application.Models.Response.Products;

public class GetProductsResponse : ErrorResponse
{
    public PagingDto Paging { get; set; }
    public List<GetProductData> Data { get; set; }
}

public class GetProductData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
