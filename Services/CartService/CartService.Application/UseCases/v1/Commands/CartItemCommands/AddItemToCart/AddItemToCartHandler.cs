using CartService.Application.Models.Response.CartItems;
using CartService.Application.Services.GrpcService;
using CartService.Domain.Abstraction;
using CartService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart;

public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, AddItemToCartResponse>
{
    private readonly ILogger<AddItemToCartHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductService _productService;

    public AddItemToCartHandler
    (
        ILogger<AddItemToCartHandler> logger,
        IUnitOfWork unitOfWork,
        IProductService productService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _productService = productService;
    }

    public async Task<AddItemToCartResponse> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(AddItemToCartHandler)} => ";
        var response = new AddItemToCartResponse { Status = ResponseStatusCode.OK.ToInt() };
        _logger.LogInformation($"{functionName}");
        var payload = request.Payload;
        var cartId = request.CartId;
        var isUpdatedQuantity = false;
        try
        {
            var cart = await _unitOfWork.Cart.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == cartId, cancellationToken);

            if (cart == null)
            {
                _logger.LogWarning($"{functionName} Cart does not exist");
                return CreateErrorResponse(ResponseStatusCode.BadRequest, "Cart does not exist");
            }

            var cartItem = await _unitOfWork.CartItem.GetQueryable()
                .FirstOrDefaultAsync(x => x.ProductId == payload.ProductId && x.CartId == cartId,
                    cancellationToken);

            var productResponse =
                   await _productService.UpdateProductQuantity(payload.ProductId, payload.Quantity);

            if (productResponse.Status != ResponseStatusCode.OK.ToInt())
            {
                _logger.LogWarning($"{functionName} {productResponse.ErrorMessage}");
                return CreateErrorResponse((ResponseStatusCode)productResponse.Status, productResponse.ErrorMessage);
            }

            isUpdatedQuantity = true;
            var productPrice = decimal.Parse(productResponse.Product.Price);
            cart.TotalPrice += payload.Quantity * productPrice;

            if (cartItem != null)
            {
                cartItem.Quantity += payload.Quantity;
                await _unitOfWork.CartItem.UpdateAsync(cartItem, cancellationToken);
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = payload.ProductId,
                    Quantity = payload.Quantity
                };
                await _unitOfWork.CartItem.AddAsync(cartItem, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Error occurred => {ex.Message}");
            
            // Rollback the product quantity
            if (isUpdatedQuantity)
            {
                await _productService.UpdateProductQuantity(payload.ProductId, -payload.Quantity);
            }
            return CreateErrorResponse(ResponseStatusCode.InternalServerError, $"An error has occurred");
        }

        return response;
    }

    private AddItemToCartResponse CreateErrorResponse(ResponseStatusCode statusCode, string errorMessage)
    {
        return new AddItemToCartResponse
        {
            Status = statusCode.ToInt(),
            ErrorMessage = errorMessage
        };
    }
}