using CartService.Application.Models.Response.CartItems;
using CartService.Domain.Abstraction;
using CartService.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Grpc.Protos;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart;

public class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, CreateCartItemResponse>
{
    private readonly ILogger<AddItemToCartHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Services.GrpcService.ProductService _productService;

    public AddItemToCartHandler(ILogger<AddItemToCartHandler> logger, IUnitOfWork unitOfWork,
        Services.GrpcService.ProductService productService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _productService = productService;
    }

    public async Task<CreateCartItemResponse> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(AddItemToCartHandler)} => ";
        var response = new CreateCartItemResponse { Status = ResponseStatusCode.OK.ToInt() };
        _logger.LogInformation($"{functionName}");

        try
        {
            var cart = await _unitOfWork.Cart.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == request.CartId, cancellationToken);

            if (cart == null)
            {
                _logger.LogWarning($"{functionName} Cart does not exist");
                return CreateErrorResponse(ResponseStatusCode.BadRequest, "Cart does not exist");
            }

            var cartItem = await _unitOfWork.CartItem.GetQueryable()
                .FirstOrDefaultAsync(x => x.ProductId == request.Payload.ProductId && x.CartId == request.CartId,
                    cancellationToken);

            ProductModelResponse productResponse =
                   await _productService.UpdateProductQuantity(request.Payload.ProductId, request.Payload.Quantity);

            if (productResponse.Status != ResponseStatusCode.OK.ToInt())
            {
                _logger.LogWarning($"{functionName} {productResponse.ErrorMessage}");
                return CreateErrorResponse((ResponseStatusCode)productResponse.Status, productResponse.ErrorMessage);
            }

            var productPrice = decimal.Parse(productResponse.Product.Price);
            cart.TotalPrice += request.Payload.Quantity * productPrice;

            if (cartItem != null)
            {
                cartItem.Quantity += request.Payload.Quantity;
                await _unitOfWork.CartItem.UpdateAsync(cartItem, cancellationToken);
            }
            else
            {
                cartItem = new CartItem
                {
                    CartId = request.CartId,
                    ProductId = request.Payload.ProductId,
                    Quantity = request.Payload.Quantity
                };
                await _unitOfWork.CartItem.AddAsync(cartItem, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Error occurred => {ex.Message}");
            return CreateErrorResponse(ResponseStatusCode.InternalServerError, $"An error has occurred");
        }

        return response;
    }

    private CreateCartItemResponse CreateErrorResponse(ResponseStatusCode statusCode, string errorMessage)
    {
        return new CreateCartItemResponse
        {
            Status = statusCode.ToInt(),
            ErrorMessage = errorMessage
        };
    }
}