using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Models.Response.Products;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProduct;
using ProductService.Domain.Abstraction;
using Shared.CommonExtension;
using Shared.Enums;
using Shared.Models.Response;

namespace ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProductQuantity;

public class UpdateProductQuantityHandler : IRequestHandler<UpdateProductQuantityCommand, UpdateProductQuantityResponse>
{
    private readonly ILogger<UpdateProductQuantityHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductQuantityHandler
    (
        ILogger<UpdateProductQuantityHandler> logger,
        IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateProductQuantityResponse> Handle(UpdateProductQuantityCommand request,
        CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateProductQuantityHandler)} => ProductId = {payload.Id} =>";
        var response = new UpdateProductQuantityResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            var productId = payload.Id;
            var queryable = _unitOfWork.Product.GetQueryable();

            var product = await queryable.Where(x => x.Id == productId && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                _logger.LogWarning($"{functionName} Product does not exists");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

                return response;
            }

            if (product.Quantity - payload.Quantity < 0)
            {
                _logger.LogWarning($"{functionName} Product quantity is not enough");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

                return response;
            }

            product.Quantity -= payload.Quantity;
            await _unitOfWork.Product.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            response.Data = new ProductDataResponse
            {
                Id = product.Id,
                Name = product.Name,
                Quantity = product.Quantity,
                Price = product.Price
            };
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            // $"{functionName} Some error has occurred!";
            //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

            return response;
        }

        return response;
    }
}