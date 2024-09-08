using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Models.Response.Products;
using ProductService.Domain.Abstraction;
using Shared.CommonExtension;
using Shared.Constants;
using Shared.Enums;
using Shared.Models.Response;

namespace ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductByIds;

public class GetProductByIdsHandler : IRequestHandler<GetProductByIdsQuery, GetProductsByIdsResponse>
{
    private readonly ILogger<GetProductByIdsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public GetProductByIdsHandler(ILogger<GetProductByIdsHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetProductsByIdsResponse> Handle(GetProductByIdsQuery request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(GetProductByIdsHandler)} => ";
        var productIds = request.Ids;
        var response = new GetProductsByIdsResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            var productData = await (from product in _unitOfWork.Product.GetQueryable()
                    where productIds.Contains(product.Id)
                    select new ProductDataResponse
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = product.Quantity,
                    })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            
            response.Data = productData;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetProductByIdsHandler)} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
            return response;
        }
    }
}