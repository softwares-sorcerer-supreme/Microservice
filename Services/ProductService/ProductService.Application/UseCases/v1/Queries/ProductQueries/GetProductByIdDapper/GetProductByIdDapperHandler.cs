using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Models.Response.Products;
using ProductService.Domain.Abstraction.Repositories;
using ProductService.Domain.Entities;
using Shared.CommonExtension;
using Shared.Constants;
using Shared.Enums;

namespace ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductByIdDapper;

public class GetProductByIdDapperHandler : IRequestHandler<GetProductByIdDapperQuery, GetProductByIdResponse>
{
    private readonly ILogger<GetProductByIdDapperHandler> _logger;
    private readonly IReadOnlyRepository _readOnlyRepository;

    public GetProductByIdDapperHandler
    (
        ILogger<GetProductByIdDapperHandler> logger,
        IReadOnlyRepository readOnlyRepository
    )
    {
        _logger = logger;
        _readOnlyRepository = readOnlyRepository;
    }

    public async Task<GetProductByIdResponse> Handle(GetProductByIdDapperQuery request, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(GetProductByIdDapperHandler)} => ";
        var productId = request.Id;
        var response = new GetProductByIdResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        _logger.LogInformation($"{functionName}");

        try
        {
            const string query = "SELECT * FROM \"Products\" WHERE \"Id\" = @Id";
            var product = await _readOnlyRepository.QueryFirstOrDefaultAsync<Product>(query, new { Id = productId }, cancellationToken: cancellationToken);

            response.Data = new ProductDataResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity
            };

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GetProductByIdDapperHandler)} Has error => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
            return response;
        }
    }
}