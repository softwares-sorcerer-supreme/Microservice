using CartService.Application.Models.Dtos;
using Shared.Models.Response;

namespace CartService.Application.Models.Response.CartItems;

public class GetItemsByCartIdResponse : BaseResponse
{
    public GetItemByCartData Data { get; set; }
}

public class GetItemByCartData
{
    public List<ItemDatas> Items { get; set; }
    public decimal TotalPrice { get; set; }
}

public class ItemDatas : ProductDto
{
    public int Quantity { get; set; }
}