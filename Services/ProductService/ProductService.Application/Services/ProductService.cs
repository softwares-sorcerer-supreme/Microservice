using System.Globalization;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Extensions;
using ProductService.Application.Grpc.Protos;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProductQuantity;
using ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductById;
using ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductByIds;
using Shared.CommonExtension;
using Shared.Models.Response;

namespace ProductService.Application.Services;

public class ProductService : ProductProtoService.ProductProtoServiceBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductService> _logger;
    
    public ProductService(IMediator mediator, ILogger<ProductService> logger)
    {
        _mediator = mediator;
        _logger = logger;    
    }

    public override async Task<GetProductsByIdsResponse> GetProductsByIds(GetProductsByIdsRequest request, ServerCallContext context)
    {
        var strProductIds = request.ProductIds;
        var productIds = strProductIds.Select(id => new Guid(id)).ToList();
        var getProductsByIdsResponse = await _mediator.Send(new GetProductByIdsQuery(productIds), context.CancellationToken);

        var productModels = getProductsByIdsResponse.Data.Select(product => product.ToProductModel()).ToList();
        
        var response = new GetProductsByIdsResponse
        {
            Products =
            {
                productModels
            },
            Status = getProductsByIdsResponse.Status,
            ErrorMessage = getProductsByIdsResponse.ErrorMessage,
            ErrorMessageCode = getProductsByIdsResponse.ErrorMessageCode
        };

        return response;
    }

    public override async Task<ProductModelResponse> UpdateProductQuantity(UpdateProductQuantityRequest request, ServerCallContext context)
    {
        var response = await _mediator.Send(new UpdateProductQuantityCommand(request), context.CancellationToken);
        
        var getProductsByIdResponse = new ProductModelResponse
        {
            Status = response.Status,
            ErrorMessage = response.ErrorMessage,
            ErrorMessageCode = response.ErrorMessageCode
        };

        if (response.Status != ResponseStatusCode.OK.ToInt())
        {
            return getProductsByIdResponse;
        }

        var productModel = response.Data.ToProductModel();
        getProductsByIdResponse.Product = productModel;
        
        return getProductsByIdResponse;
    }
    
    public override async Task<ProductModelResponse> GetProductsById(GetProductsByIdRequest request, ServerCallContext context)
    {
        var productId = new Guid(request.ProductId);
        var response = await _mediator.Send(new GetProductByIdQuery(productId), context.CancellationToken);
        var getProductsByIdResponse = new ProductModelResponse
        {
            Status = response.Status,
            ErrorMessage = response.ErrorMessage,
            ErrorMessageCode = response.ErrorMessageCode
        };

        if (response.Status != ResponseStatusCode.OK.ToInt())
        {
            return getProductsByIdResponse;
        }

        var productModel = response.Data.ToProductModel();
        getProductsByIdResponse.Product = productModel;

        return getProductsByIdResponse;
    }
    
}