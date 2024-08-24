using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Grpc.Protos;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProduct;
using ProductService.Domain.Abstraction;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProductQuantity;

public class UpdateProductQuantityHandler : IRequestHandler<UpdateProductQuantityCommand, UpdateProductQuantityResponse>
{
    private readonly ILogger<UpdateProductHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductQuantityHandler(
        ILogger<UpdateProductHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateProductQuantityResponse> Handle(UpdateProductQuantityCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateProductQuantityHandler)} => ProductId = ${payload.Id}";
        var response = new UpdateProductQuantityResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            var productId = new Guid(payload.Id);
            var queryable = _unitOfWork.Product.GetQueryable();

            var product = await queryable.Where(x => x.Id == productId && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                _logger.LogWarning($"{functionName} Product does not exists");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                response.ErrorMessage = "Product does not exists";
                //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

                return response;
            }
            
            if(product.Quantity - payload.Quantity < 0)
            {
                _logger.LogWarning($"{functionName} Product quantity is not enough");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                response.ErrorMessage = "Product quantity is not enough";
                //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

                return response;
            }
            
            product.Quantity -= payload.Quantity;
            await _unitOfWork.Product.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessage = $"{functionName} Some error has occurred!";
            //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

            return response;
        }

        return response;
    }
}
