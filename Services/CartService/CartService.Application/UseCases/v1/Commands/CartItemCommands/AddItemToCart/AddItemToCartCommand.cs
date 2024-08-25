using CartService.Application.Models.Request.CartItems;
using CartService.Application.Models.Response.CartItems;
using MediatR;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart;

public class AddItemToCartCommand : IRequest<AddItemToCartResponse>
{
    public Guid CartId { get; set; }
    public AddItemToCartRequest Payload { get; set; }
    public AddItemToCartCommand(Guid cartId, AddItemToCartRequest payload)
    {
        CartId = cartId;
        Payload = payload;
    }
}
