using CartService.Application.Models.Request.CartItems;
using CartService.Application.Models.Response.CartItems;
using MediatR;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart;

public class AddItemToCartCommand : IRequest<CreateCartItemResponse>
{
    public Guid CartId { get; set; }
    public CreateCartItemRequest Payload { get; set; }
    public AddItemToCartCommand(CreateCartItemRequest payload, Guid cartId)
    {
        CartId = cartId;
        Payload = payload;
    }
}
