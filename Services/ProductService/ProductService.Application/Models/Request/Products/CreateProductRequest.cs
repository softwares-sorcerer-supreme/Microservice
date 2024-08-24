namespace ProductService.Application.Models.Request.Products;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
