using CartService.Application.Models.Response.CartItems;
using CartService.Domain.Abstraction;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace CartService.Application.UseCases.v1.Commands.CartItemCommands.DeleteItemToCart;

public class DeleteItemToCartHandler : IRequestHandler<DeleteItemToCartCommand, RemoveCartItemResponse>
{
    private readonly ILogger<DeleteItemToCartHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteItemToCartHandler(ILogger<DeleteItemToCartHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<RemoveCartItemResponse> Handle(DeleteItemToCartCommand request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(DeleteItemToCartHandler)} => ";
        var id = request.Id;
        var response = new RemoveCartItemResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            var queryable = _unitOfWork.Cart.GetQueryable();

            var product = await queryable.Where(x => x.Id == id )
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                _logger.LogWarning($"{functionName} Product does not exists");
                response.Status = ResponseStatusCode.NotFound.ToInt();
                response.ErrorMessage = "Product does not exists";
                //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

                return response;
            }
            
            
            // await _unitOfWork.Product.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessage = $"{functionName} Some error has occured!";
            //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

            return response;
        }

        return response;
    }
}