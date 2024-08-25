using CartService.Application.Models.Response.CartItems;
using CartService.Application.Services.GrpcService;
using CartService.Domain.Abstraction;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.CommonExtension;
using Shared.Models.Response;
using CartService.Domain.Entities;
using ProductService.Application.Grpc.Protos;

namespace CartService.Application.UseCases.v1.Queries.CartItemQueries.GetItemsByCartId;

public class GetItemsByCartIdHandler : IRequestHandler<GetItemsByCartIdQuery, GetItemsByCartIdResponse>
{
    private readonly ILogger<GetItemsByCartIdHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductService _productService;

    public GetItemsByCartIdHandler
    (
        ILogger<GetItemsByCartIdHandler> logger,
        IUnitOfWork unitOfWork,
        IProductService productService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _productService = productService;
    }

    public async Task<GetItemsByCartIdResponse> Handle(GetItemsByCartIdQuery request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(GetItemsByCartIdHandler)} => ";
        _logger.LogInformation($"{functionName}");
        var response = new GetItemsByCartIdResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };
        var cartId = request.CartId;;
        
        try
        {
            var cart = await _unitOfWork.Cart.GetQueryable().FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);
            if (cart == null)
            {
                _logger.LogWarning($"{functionName} Cart does not exist");
                return CreateErrorResponse(ResponseStatusCode.NotFound, "Cart does not exist");
            }

            var cartItems = await GetCartItemsByCartIdAsync(request.CartId, cancellationToken);
            if (cartItems.Count == 0)
            {
                return CreateResponseWithTotalPrice(cart.TotalPrice);
            }

            var productResponse = await _productService.GetProductsByIds(cartItems.Select(ci => ci.ProductId).ToList());
            if (productResponse.Status != ResponseStatusCode.OK.ToInt() || productResponse.Products.Count == 0)
            {
                return CreateErrorResponse((ResponseStatusCode)productResponse.Status, productResponse.ErrorMessage);
            }

            var items = MapProductsToItemDatas(cartItems, productResponse.Products.ToList());
            return CreateResponseWithItems(cart.TotalPrice, items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Error in GetItemsByCartIdHandler");
            return CreateErrorResponse(ResponseStatusCode.InternalServerError, "Something went wrong");
        }
    }

    private async Task<List<CartItem>> GetCartItemsByCartIdAsync(Guid cartId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.CartItem.GetQueryable()
            .Where(ci => ci.CartId == cartId)
            .ToListAsync(cancellationToken);
    }

    private List<ItemDatas> MapProductsToItemDatas(List<CartItem> cartItems, List<ProductModel> products)
    {
        return products.Select(p => new ItemDatas
        {
            Id = new Guid(p.Id),
            Name = p.Name,
            Price = decimal.Parse(p.Price),
            Quantity = cartItems.FirstOrDefault(ci => ci.ProductId == new Guid(p.Id))?.Quantity ?? 0
        }).ToList();
    }

    private GetItemsByCartIdResponse CreateResponseWithTotalPrice(decimal totalPrice)
    {
        return new GetItemsByCartIdResponse
        {
            Status = ResponseStatusCode.OK.ToInt(),
            Data = new GetItemByCartData
            {
                Items = new(),
                TotalPrice = totalPrice
            }
        };
    }

    private GetItemsByCartIdResponse CreateResponseWithItems(decimal totalPrice, List<ItemDatas> items)
    {
        return new GetItemsByCartIdResponse
        {
            Status = ResponseStatusCode.OK.ToInt(),
            Data = new GetItemByCartData
            {
                Items = items,
                TotalPrice = totalPrice
            }
        };
    }

    private GetItemsByCartIdResponse CreateErrorResponse(ResponseStatusCode status, string errorMessage)
    {
        return new GetItemsByCartIdResponse
        {
            Status = status.ToInt(),
            ErrorMessage = errorMessage
        };
    }
}
