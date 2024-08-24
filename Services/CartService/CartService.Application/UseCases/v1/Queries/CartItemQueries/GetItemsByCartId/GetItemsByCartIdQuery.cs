using CartService.Application.Models.Response.CartItems;
using MediatR;

namespace CartService.Application.UseCases.v1.Queries.CartItemQueries.GetItemsByCartId;

public class GetItemsByCartIdQuery : IRequest<GetItemsByCartIdResponse>
{
    public Guid CartId { get; set; }
    public GetItemsByCartIdQuery(Guid cartId)
    {
        CartId = cartId;
    }
}
