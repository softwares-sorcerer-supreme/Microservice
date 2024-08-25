namespace CartService.Application.Models.Request.CartItems;

public class AddItemToCartRequest
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
