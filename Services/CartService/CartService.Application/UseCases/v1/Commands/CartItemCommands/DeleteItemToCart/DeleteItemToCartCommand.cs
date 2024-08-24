using CartService.Application.Models.Response.CartItems;
using MediatR;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.DeleteItemToCart;

public class DeleteItemToCartCommand : IRequest<RemoveCartItemResponse>
{
    public Guid Id { get; set; }
    public DeleteItemToCartCommand(Guid id)
    {
        Id = id;
    }
}