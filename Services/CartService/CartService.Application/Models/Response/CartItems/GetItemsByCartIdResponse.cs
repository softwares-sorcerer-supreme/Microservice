using CartService.Application.Models.Dtos;
using Shared.Models.Response;

namespace CartService.Application.Models.Response.CartItems;

public record GetItemsByCartIdResponse : BaseResponse
{
    public GetItemByCartData Data { get; set; }
}

public record GetItemByCartData
{
    public List<ItemDatas> Items { get; set; }
    public decimal TotalPrice { get; set; }
}

public record ItemDatas : ProductDto
{
    public int Quantity { get; set; }
}