namespace ProductService.Application.Models.Request.Products;

public class UpdateProductRequest
{
    public Guid Id { get; set; }   
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
