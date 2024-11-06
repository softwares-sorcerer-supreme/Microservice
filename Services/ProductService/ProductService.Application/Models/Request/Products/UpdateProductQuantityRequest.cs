namespace ProductService.Application.Models.Request.Products;

public record UpdateProductQuantityRequest
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}