namespace ProductService.Application.Models.Request.Products;

public record UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}