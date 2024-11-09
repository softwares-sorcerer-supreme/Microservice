using AutoFixture;
using Microsoft.Extensions.Logging;
using MockQueryable.EntityFrameworkCore;
using Moq;
using ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductByIds;
using ProductService.Domain.Abstraction;
using ProductService.Domain.Entities;
using Shared.Enums;
using Xunit;

namespace ProductService.Test.UseCases.v1.Queries.ProductQueries.GetProductByIds;

public class GetProductByIdsHandlerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly GetProductByIdsHandler _handler;
    private readonly CancellationToken _cancellationToken = new CancellationToken();
    private readonly Mock<ILogger<GetProductByIdsHandler>> _logger;

    public GetProductByIdsHandlerTest()
    {
        _fixture = new Fixture();
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger<GetProductByIdsHandler>>();
        _handler = new GetProductByIdsHandler(_logger.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ProductsExist_ReturnsOkResponseWithProducts()
    {
        // Arrange
        var productIds = _fixture.CreateMany<Guid>(3).ToList();
        var existingProducts = _fixture.Build<Product>()
            .With(x => x.Id, productIds[0])
            .With(x => x.IsDeleted, false)
            .CreateMany(3)
            .ToList();

        var productQueryable = existingProducts.AsQueryable().BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(productQueryable);

        var query = new GetProductByIdsQuery(productIds);

        // Act
        var response = await _handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.OK, response.Status);
        Assert.NotNull(response.Data);
        Assert.Equal(3, response.Data.Count);
        Assert.All(response.Data, product => Assert.Contains(productIds, id => id == product.Id));
        Assert.Empty(response.ErrorMessageCode); // Check that there is no error message
    }

    [Fact]
    public async Task Handle_NoProductsFound_ReturnsOkResponseWithEmptyList()
    {
        // Arrange
        var productIds = _fixture.CreateMany<Guid>(3).ToList();

        var productQueryable = new List<Product>().AsQueryable().BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(productQueryable);

        var query = new GetProductByIdsQuery(productIds);

        // Act
        var response = await _handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.OK, response.Status);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);
        Assert.Empty(response.ErrorMessageCode); // Check that there is no error message
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsInternalServerErrorResponse()
    {
        // Arrange
        var productIds = _fixture.CreateMany<Guid>(3).ToList();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Throws(new Exception("Test exception"));

        var query = new GetProductByIdsQuery(productIds);

        // Act
        var response = await _handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.InternalServerError, response.Status);
        Assert.Equal("Something went wrong", response.ErrorMessageCode);
        Assert.Null(response.Data);
    }
}