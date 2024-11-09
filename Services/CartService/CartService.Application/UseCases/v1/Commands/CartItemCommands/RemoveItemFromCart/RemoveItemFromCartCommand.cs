using CartService.Application.Models.Request.CartItems;
using CartService.Application.Models.Response.CartItems;
using MediatR;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.RemoveItemFromCart;

public class RemoveItemFromCartCommand : IRequest<RemoveItemFromCartResponse>
{
    public RemoveItemFromCartRequest Payload { get; set; }

    public RemoveItemFromCartCommand(RemoveItemFromCartRequest payload)
    {
        Payload = payload;
    }
}