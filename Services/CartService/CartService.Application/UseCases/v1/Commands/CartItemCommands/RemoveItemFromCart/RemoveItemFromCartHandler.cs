using CartService.Application.Models.Response.CartItems;
using CartService.Application.Services.GrpcService;
using CartService.Domain.Abstraction;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.RemoveItemFromCart;

public class RemoveItemFromCartHandler : IRequestHandler<RemoveItemFromCartCommand, RemoveItemFromCartResponse>
{
    private readonly ILogger<RemoveItemFromCartHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductService _productService;

    public RemoveItemFromCartHandler
    (
        ILogger<RemoveItemFromCartHandler> logger,
        IUnitOfWork unitOfWork,
        IProductService productService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _productService = productService;
    }

    public async Task<RemoveItemFromCartResponse> Handle(RemoveItemFromCartCommand request,
        CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(RemoveItemFromCartHandler)} => ";
        var payload = request.Payload;
        var response = new RemoveItemFromCartResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };
        var isUpdatedQuantity = false;
        _logger.LogInformation($"{functionName}");
        var quantity = 0;
        try
        {
            var queryable = _unitOfWork.Cart.GetQueryable();

            var cart = await queryable.Where(x => x.Id == payload.CartId)
                .FirstOrDefaultAsync(cancellationToken);

            if (cart == null)
            {
                _logger.LogWarning($"{functionName} Cart does not exist");
                return CreateErrorResponse(ResponseStatusCode.BadRequest, "Cart does not exist");
            }

            var cartItem = await _unitOfWork.CartItem.GetQueryable()
                .FirstOrDefaultAsync(x => x.CartId == payload.CartId && x.ProductId == payload.ProductId,
                    cancellationToken);

            if (cartItem == null)
            {
                _logger.LogWarning($"{functionName} CartItem does not exist");
                return CreateErrorResponse(ResponseStatusCode.BadRequest, "CartItem does not exist");
            }

            quantity = cartItem.Quantity;
            var productResponse = await _productService.UpdateProductQuantity(cartItem.ProductId, -quantity);
            if (productResponse.Status != ResponseStatusCode.OK.ToInt())
            {
                _logger.LogWarning($"{functionName} {productResponse.ErrorMessage}");
                return CreateErrorResponse((ResponseStatusCode)productResponse.Status, productResponse.ErrorMessage);
            }
            
            isUpdatedQuantity = true;
            var productPrice = decimal.Parse(productResponse.Product.Price);
            cart.TotalPrice -= quantity * productPrice;
            
            await _unitOfWork.CartItem.RemoveAsync(cartItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessage = $"{functionName} Some error has occured!";
            //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

            // Rollback the product quantity
            if (isUpdatedQuantity)
            {
                await _productService.UpdateProductQuantity(payload.ProductId, quantity);
            }

            return response;
        }

        return response;
    }

    private RemoveItemFromCartResponse CreateErrorResponse(ResponseStatusCode statusCode, string errorMessage)
    {
        return new RemoveItemFromCartResponse
        {
            Status = statusCode.ToInt(),
            ErrorMessage = errorMessage
        };
    }
}