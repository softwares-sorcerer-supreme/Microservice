using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Application.Grpc.Protos;
using ProductService.Application.Models.Response.Products;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProductQuantity;
using ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductById;
using ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductByIds;
using Shared.CommonExtension;
using Shared.Constants;
using Shared.Enums;
using Shared.Models.Response;
using Xunit;
using Assert = Xunit.Assert;

namespace ProductService.Test.Services;

public class ProductServiceTests
{
    private readonly Application.Services.ProductService _service;
    private readonly CancellationToken _cancellationToken;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<Application.Services.ProductService>> _loggerMock;
    private readonly ServerCallContext _mockContext;

    public ProductServiceTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<Application.Services.ProductService>>();
        _service = new Application.Services.ProductService(_mediatorMock.Object, _loggerMock.Object);
        _cancellationToken = new CancellationToken();
        _mockContext = new Mock<ServerCallContext>().Object;
    }

    [Fact]
    public async Task GetProductsByIds_ValidRequest_ReturnsProductModels()
    {
        // Arrange
        var request = new GetProductsByIdsRequest
        {
            ProductIds = { "c1f7435d-02cb-462b-a1ff-9c908f317c0d", "f4a5f543-8c66-4b6c-b5a0-8bfae39cfbd8" }
        };

        var productIds = request.ProductIds.Select(id => new Guid(id)).ToList();
        var responseFromMediator = new Application.Models.Response.Products.GetProductsByIdsResponse
        {
            Data = new List<ProductDataResponse>
            {
                new ProductDataResponse { Id = productIds[0], Name = "Product1", Quantity = 10, Price = 20.0M },
                new ProductDataResponse { Id = productIds[1], Name = "Product2", Quantity = 5, Price = 30.0M }
            },
            Status = ResponseStatusCode.OK.ToInt()
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetProductByIdsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseFromMediator);

        // Act
        var response = await _service.GetProductsByIds(request, _mockContext);

        // Assert
        Assert.Equal(ResponseStatusCode.OK.ToInt(), response.Status);
        Assert.Equal(2, response.Products.Count);
        Assert.Contains(response.Products, p => p.Id == productIds[0].ToString());
        Assert.Contains(response.Products, p => p.Id == productIds[1].ToString());
    }

    [Fact]
    public async Task GetProductsByIds_NoProducts_ReturnsEmptyResponse()
    {
        // Arrange

        var request = new GetProductsByIdsRequest
        {
            ProductIds = { "c1f7435d-02cb-462b-a1ff-9c908f317c0d" }
        };

        var responseFromMediator = new ProductService.Application.Models.Response.Products.GetProductsByIdsResponse
        {
            Data = new List<ProductDataResponse>(),
            Status = ResponseStatusCode.OK.ToInt()
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetProductByIdsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseFromMediator);

        // Act
        var response = await _service.GetProductsByIds(request, _mockContext);

        // Assert
        Assert.Equal(ResponseStatusCode.OK.ToInt(), response.Status);
        Assert.Empty(response.Products);
    }

    #region UpdateProductQuantity

    [Fact]
    public async Task UpdateProductQuantity_ValidRequest_ReturnsUpdatedProductModel()
    {
        // Arrange
        var request = new UpdateProductQuantityRequest
        {
            Id = "c1f7435d-02cb-462b-a1ff-9c908f317c0d",
            Quantity = 5
        };

        var responseFromMediator = new UpdateProductQuantityResponse
        {
            Status = ResponseStatusCode.OK.ToInt(),
            Data = new ProductDataResponse
            {
                Id = new Guid(request.Id),
                Name = "Product1",
                Quantity = 5,
                Price = 20.0M
            }
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<UpdateProductQuantityCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseFromMediator);

        // Act
        var response = await _service.UpdateProductQuantity(request, _mockContext);

        // Assert
        Assert.Equal(ResponseStatusCode.OK.ToInt(), response.Status);
        Assert.Equal(request.Id, response.Product.Id);
        Assert.Equal(5, response.Product.Quantity);
    }

    [Fact]
    public async Task UpdateProductQuantity_ProductNotExists_ReturnsError()
    {
        // Arrange
        var request = new UpdateProductQuantityRequest
        {
            Id = "c1f7435d-02cb-462b-a1ff-9c908f317c0d",
            Quantity = 5
        };

        var responseFromMediator = new UpdateProductQuantityResponse
        {
            Status = ResponseStatusCode.BadRequest.ToInt(),
            ErrorMessageCode = ResponseErrorMessageCode.ERR_PRODUCT_0001
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<UpdateProductQuantityCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseFromMediator);

        // Act
        var response = await _service.UpdateProductQuantity(request, _mockContext);

        // Assert
        Assert.Equal(ResponseStatusCode.BadRequest.ToInt(), response.Status);
        Assert.Equal(ResponseErrorMessageCode.ERR_PRODUCT_0001, response.ErrorMessageCode);
    }

    #endregion

    #region GetProductsById

    [Fact]
    public async Task GetProductsById_ValidRequest_ReturnsProductModel()
    {
        // Arrange
        var request = new GetProductsByIdRequest
        {
            ProductId = "c1f7435d-02cb-462b-a1ff-9c908f317c0d"
        };

        var productId = new Guid(request.ProductId);
        var responseFromMediator = new GetProductByIdResponse
        {
            Data = new ProductDataResponse
            {
                Id = productId,
                Name = "Product1",
                Quantity = 10,
                Price = 20.0M
            },
            Status = ResponseStatusCode.OK.ToInt()
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseFromMediator);

        // Act
        var response = await _service.GetProductsById(request, _mockContext);

        // Assert
        Assert.Equal(ResponseStatusCode.OK.ToInt(), response.Status);
        Assert.Equal(request.ProductId, response.Product.Id);
        Assert.Equal("Product1", response.Product.Name);
    }

    [Fact]
    public async Task GetProductsById_ProductNotExists_ReturnsError()
    {
        // Arrange
        var request = new GetProductsByIdRequest
        {
            ProductId = "c1f7435d-02cb-462b-a1ff-9c908f317c0d"
        };

        var responseFromMediator = new GetProductByIdResponse
        {
            Status = ResponseStatusCode.BadRequest.ToInt(),
            ErrorMessageCode = ResponseErrorMessageCode.ERR_PRODUCT_0001
        };

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseFromMediator);

        // Act
        var response = await _service.GetProductsById(request, _mockContext);

        // Assert
        Assert.Equal(ResponseStatusCode.BadRequest.ToInt(), response.Status);
        Assert.Equal(ResponseErrorMessageCode.ERR_PRODUCT_0001, response.ErrorMessageCode);
    }

    #endregion
}