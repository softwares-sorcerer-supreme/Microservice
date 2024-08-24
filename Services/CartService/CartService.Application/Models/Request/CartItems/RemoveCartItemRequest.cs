namespace CartService.Application.Models.Request.CartItems;

public class RemoveCartItemRequest
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
}