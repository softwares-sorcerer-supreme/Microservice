using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Models.Response.Products;
using ProductService.Domain.Abstraction;
using Shared.CommonExtension;
using Shared.Enums;

namespace ProductService.Application.UseCases.v1.Commands.ProductCommands.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponse>
{
    private readonly ILogger<DeleteProductHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductHandler(ILogger<DeleteProductHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteProductResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(DeleteProductHandler)} =>";
        var id = request.Id;
        var response = new DeleteProductResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            var queryable = _unitOfWork.Product.GetQueryable();

            var product = await queryable.Where(x => x.Id == id && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                _logger.LogWarning($"{functionName} Product does not exists");
                response.Status = ResponseStatusCode.NotFound.ToInt();
                // response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

                return response;
            }

            product.IsDeleted = true;

            await _unitOfWork.Product.UpdateAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            // response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

            return response;
        }

        return response;
    }
}