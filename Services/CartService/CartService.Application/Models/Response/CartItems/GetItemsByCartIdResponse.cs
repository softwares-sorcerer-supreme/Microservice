using CartService.Application.Models.Dtos;
using Shared.Models.Response;

namespace CartService.Application.Models.Response.CartItems;

public class GetItemsByCartIdResponse : ApiResponse<GetItemByCartData>;

public record GetItemByCartData
{
    public List<ItemData> Items { get; set; }
    public decimal TotalPrice { get; set; }
}

public record ItemData : ProductDto
{
    public int Quantity { get; set; }
}