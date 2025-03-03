using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Extensions;
using ProductService.Application.Grpc.Protos;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProductQuantity;
using ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductById;
using ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductByIds;
using Shared.CommonExtension;
using Shared.Constants;
using Shared.Enums;

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

    public override async Task<GetProductsByIdsResponse> GetProductsByIds(GetProductsByIdsRequest request,
        ServerCallContext context)
    {
        const string functionName = $"{nameof(ProductService)} => GetProductsByIds => ";
        _logger.LogInformation(
            $"{functionName} Request received with ProductIds: {string.Join(", ", request.ProductIds)}");

        var response = new GetProductsByIdsResponse();

        try
        {
            var productIds = request.ProductIds.Select(id => new Guid(id)).ToList();
            var mediatorResponse = await _mediator.Send(new GetProductByIdsQuery(productIds), context.CancellationToken);

            if (mediatorResponse.Status != ResponseStatusCode.OK.ToInt())
            {
                _logger.LogWarning($"{functionName} Failed to get products.");
                response.Status = mediatorResponse.Status;
                response.ErrorMessageCode = mediatorResponse.ErrorMessageCode;
                return response;
            }

            var productModels = mediatorResponse.Data.Select(product => product.ToProductModel()).ToList();
            response.Products.AddRange(productModels);
            response.Status = mediatorResponse.Status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Exception occurred: {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            // "An error occurred while processing the request.";
        }

        return response;
    }

    public override async Task<ProductModelResponse> UpdateProductQuantity(UpdateProductQuantityRequest request,
        ServerCallContext context)
    {
        const string functionName = $"{nameof(ProductService)} => UpdateProductQuantity => ";
        _logger.LogInformation($"{functionName} Request received with Id: {request.Id}, Quantity: {request.Quantity}");

        var response = new ProductModelResponse();

        try
        {
            var id = new Guid(request.Id);
            var requestPayload = new Models.Request.Products.UpdateProductQuantityRequest
            {
                Id = id,
                Quantity = request.Quantity
            };

            var mediatorResponse = await _mediator.Send(new UpdateProductQuantityCommand(requestPayload),
                context.CancellationToken);

            response.Status = mediatorResponse.Status;
            // mediatorResponse.ErrorMessage;
            response.ErrorMessageCode = mediatorResponse.ErrorMessageCode;

            if (mediatorResponse.Status != ResponseStatusCode.OK.ToInt())
            {
                _logger.LogWarning($"{functionName} Failed to update product quantity.");
                return response;
            }

            response.Product = mediatorResponse.Data.ToProductModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Exception occurred: {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            // $"An error occurred while processing the request.";
        }

        return response;
    }

    public override async Task<ProductModelResponse> GetProductsById(GetProductsByIdRequest request,
        ServerCallContext context)
    {
        const string functionName = $"{nameof(ProductService)} => GetProductsById => ";
        _logger.LogInformation($"{functionName} Request received with ProductId: {request.ProductId}");

        var response = new ProductModelResponse();

        try
        {
            var productId = new Guid(request.ProductId);
            var mediatorResponse = await _mediator.Send(new GetProductByIdQuery(productId), context.CancellationToken);

            response.Status = mediatorResponse.Status;
            response.ErrorMessageCode = mediatorResponse.ErrorMessageCode;

            if (mediatorResponse.Status != ResponseStatusCode.OK.ToInt())
            {
                _logger.LogWarning($"{functionName} Failed to get product.");
                return response;
            }

            response.Product = mediatorResponse.Data.ToProductModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{functionName} Exception occurred: {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
        }

        return response;
    }

    //Test server streaming grpc
    public override async Task TestSeverStreaming(TestSeverStreamingRequest request,
        IServerStreamWriter<TestSeverStreamingResponse> responseStream,
        ServerCallContext context)
    {
        var listId = new List<string> { "1", "2", "3", "4", "5" };
        foreach (var id in listId)
        {
            var response = new TestSeverStreamingResponse
            {
                Id = id,
            };
            await responseStream.WriteAsync(response);
        }
    }

    //Test client streaming grpc
    public override async Task<TestSeverStreamingResponse> TestClientStreaming(
        IAsyncStreamReader<TestSeverStreamingRequest> requestStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext())
        {
            var request = requestStream.Current;
            Console.WriteLine(request.Id);
        }

        return new TestSeverStreamingResponse
        {
            Id = "1"
        };
    }
}