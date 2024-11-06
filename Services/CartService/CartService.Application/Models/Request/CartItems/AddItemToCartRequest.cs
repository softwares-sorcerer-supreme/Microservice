namespace CartService.Application.Models.Request.CartItems;

public record AddItemToCartRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
