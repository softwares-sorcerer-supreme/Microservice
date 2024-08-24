using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Models.Response.Products;
using ProductService.Domain.Abstraction;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdResponse>
{
    private readonly ILogger<GetProductByIdHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public GetProductByIdHandler(ILogger<GetProductByIdHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProductByIdResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(GetProductByIdHandler)} => ";
        var productId = request.Id;
        var response = new GetProductByIdResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            var productData = await (from product in _unitOfWork.Product.GetQueryable()
                    where product.Id == productId
                    select new GetProductData
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = product.Quantity,
                    })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if(productData == null)
            {
                _logger.LogWarning($"{functionName} Product does not exists");
                response.Status = ResponseStatusCode.BadRequest.ToInt();
                response.ErrorMessage = "Product does not exists";
                //response.ErrorMessageCode = ResponseStatusCode.BadRequest.ToInt();

                return response;
            }
            
            response.Data = productData;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetProductByIdHandler)} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessage = "Something went wrong";
            //response.ErrorMessageCode = ""
            return response;
        }
    }
}