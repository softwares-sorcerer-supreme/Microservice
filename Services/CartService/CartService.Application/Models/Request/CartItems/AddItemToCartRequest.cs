namespace CartService.Application.Models.Request.CartItems;

public class AddItemToCartRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
