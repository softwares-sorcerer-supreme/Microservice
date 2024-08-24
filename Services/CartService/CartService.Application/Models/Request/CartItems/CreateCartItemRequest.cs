namespace CartService.Application.Models.Request.CartItems;

public class CreateCartItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
