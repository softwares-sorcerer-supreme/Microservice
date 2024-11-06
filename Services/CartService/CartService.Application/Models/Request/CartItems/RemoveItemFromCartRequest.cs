namespace CartService.Application.Models.Request.CartItems;

public record RemoveItemFromCartRequest
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
}