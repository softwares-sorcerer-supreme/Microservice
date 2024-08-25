using CartService.Application.Models.Request.CartItems;
using CartService.Application.Models.Response.CartItems;
using MediatR;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart;

public class AddItemToCartCommand : IRequest<AddItemToCartResponse>
{
    public AddItemToCartRequest Payload { get; set; }
    public AddItemToCartCommand(AddItemToCartRequest payload)
    {
        Payload = payload;
    }
}
