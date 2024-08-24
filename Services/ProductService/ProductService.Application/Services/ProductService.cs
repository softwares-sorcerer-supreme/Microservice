using System.Globalization;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Grpc.Protos;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProductQuantity;
using ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductByIds;

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

        var productModels = getProductsByIdsResponse.Data.Select(product => new ProductModel
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Price = product.Price.ToString(CultureInfo.InvariantCulture),
            Quantity = product.Quantity
        }).ToList();
        
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

    public override async Task<UpdateProductQuantityResponse> UpdateProductQuantity(UpdateProductQuantityRequest request, ServerCallContext context)
    {
        var response = await _mediator.Send(new UpdateProductQuantityCommand(request), context.CancellationToken);
        return response;
    }
}