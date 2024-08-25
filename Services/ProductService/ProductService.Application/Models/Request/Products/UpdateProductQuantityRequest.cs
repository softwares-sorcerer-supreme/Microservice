namespace ProductService.Application.Models.Request.Products;

public class UpdateProductQuantityRequest
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}