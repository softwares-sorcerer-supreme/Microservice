using CartService.Application.Services.GrpcService;
using Microsoft.Extensions.Logging;
using ProductService.Application.Grpc.Protos;
using Shared.CommonExtension;
using Shared.Enums;
using Shared.Models.Response;

namespace CartService.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ProductProtoService.ProductProtoServiceClient _productProtoServiceClient;
    private readonly ILogger<ProductService> _logger;
    
    public ProductService(ProductProtoService.ProductProtoServiceClient productProtoServiceClient, ILogger<ProductService> logger)
    {
        _productProtoServiceClient = productProtoServiceClient;
        _logger = logger;
    }

    public async Task<GetProductsByIdsResponse> GetProductsByIds(List<Guid> ids)
    {
        const string functionName = $"{nameof(ProductService)} => {nameof(GetProductsByIds)} => ";
        _logger.LogInformation($"{functionName} {string.Join(",", ids)}");
        
        var response = new GetProductsByIdsResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        try
        {
            var productRequest = new GetProductsByIdsRequest
            {
                ProductIds =
                {
                    ids.Select(x => x.ToString())
                }
            };

            response = await _productProtoServiceClient.GetProductsByIdsAsync(productRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            // "Some error has occured!";
        }

        return response;
    }

    public async Task<ProductModelResponse> UpdateProductQuantity(Guid id, int quantity)
    {
        const string functionName = $"{nameof(ProductService)} => {nameof(UpdateProductQuantity)} => ";
        _logger.LogInformation($"{functionName} ProductId = {id}; Quantity = {quantity}");
        
        var response = new ProductModelResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        try
        {
            var productRequest = new UpdateProductQuantityRequest
            {
                Id = id.ToString(),
                Quantity = quantity
            };

            response = await _productProtoServiceClient.UpdateProductQuantityAsync(productRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            // "Some error has occured!";
        }

        return response;
    }

    public async Task<ProductModelResponse> GetProductsById(Guid id)
    {
        const string functionName = $"{nameof(ProductService)} => {nameof(GetProductsById)} => ";
        _logger.LogInformation($"{functionName} Id = {id}");
        var response = new ProductModelResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };

        try
        {
            var productRequest = new GetProductsByIdRequest
            {
                ProductId = id.ToString(),
            };

            response = await _productProtoServiceClient.GetProductsByIdAsync(productRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} => {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            // "Some error has occured!";
        }

        return response;
    }
}