using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Models.Response.Categories;
using ProductService.Application.Models.Response.Products;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.CreateProduct;
using ProductService.Domain.Abstraction;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
{
    private readonly ILogger<UpdateProductHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductHandler(
        ILogger<UpdateProductHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(CreateProductHandler)} Handler => ";
        var payload = request.Payload;
        var response = new UpdateProductResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            var queryable = _unitOfWork.Product.GetQueryable();

            var product = await queryable.Where(x => x.Id == payload.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                _logger.LogWarning($"{functionName} Product does not exists");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                response.ErrorMessage = "Product does not exists";
                //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

                return response;
            }

            product.Name = payload.Name;
            product.Price = payload.Price;
            product.Quantity = payload.Quantity;
            
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
