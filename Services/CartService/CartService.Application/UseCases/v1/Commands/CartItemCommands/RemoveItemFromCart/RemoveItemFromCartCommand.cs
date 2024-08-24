using CartService.Application.Models.Response.CartItems;
using MediatR;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.RemoveItemFromCart;

public class RemoveItemFromCartCommand : IRequest<RemoveItemFromCartResponse>
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public RemoveItemFromCartCommand(Guid id, Guid productId)
    {
        Id = id;
        ProductId = productId;
    }
}