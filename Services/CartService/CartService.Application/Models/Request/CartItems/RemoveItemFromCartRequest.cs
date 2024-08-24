namespace CartService.Application.Models.Request.CartItems;

public class RemoveItemFromCartRequest
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
}