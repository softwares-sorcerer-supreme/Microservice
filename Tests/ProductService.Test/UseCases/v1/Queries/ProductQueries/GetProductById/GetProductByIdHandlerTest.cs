using AutoFixture;
using Microsoft.Extensions.Logging;
using MockQueryable.EntityFrameworkCore;
using Moq;
using ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductById;
using ProductService.Domain.Abstraction;
using ProductService.Domain.Entities;
using Shared.Models.Response;
using Xunit;
using Assert = Xunit.Assert;

namespace ProductService.Test.UseCases.v1.Queries.ProductQueries.GetProductById;

public class GetProductByIdHandlerTest
{
    private readonly IFixture _fixture;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly GetProductByIdHandler _handler;
    private readonly CancellationToken _cancellationToken = new CancellationToken();
    private readonly Mock<ILogger<GetProductByIdHandler>> _logger;

    public GetProductByIdHandlerTest()
    {
        _fixture = new Fixture();
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger<GetProductByIdHandler>>();
        _handler = new GetProductByIdHandler(_logger.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ProductExists_ReturnsOkResponse()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();

        var existingProduct = _fixture.Build<Product>()
            .With(x => x.Id, productId)
            .With(x => x.IsDeleted, false)
            .Create();

        var productQueryable = new List<Product> { existingProduct }.AsQueryable().BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(productQueryable);

        var query = new GetProductByIdQuery(productId);

        // Act
        var response = await _handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.OK, response.Status);
        Assert.NotNull(response.Data);
        Assert.Equal(existingProduct.Id, response.Data.Id);
        Assert.Equal(existingProduct.Name, response.Data.Name);
        Assert.Equal(existingProduct.Price, response.Data.Price);
        Assert.Equal(existingProduct.Quantity, response.Data.Quantity);
        Assert.Empty(response.ErrorMessage); // Check that there is no error message
    }

    [Fact]
    public async Task Handle_ProductDoesNotExist_ReturnsBadRequestResponse()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();

        var productQueryable = new List<Product>().AsQueryable().BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(productQueryable);

        var query = new GetProductByIdQuery(productId);

        // Act
        var response = await _handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.BadRequest, response.Status);
        Assert.Equal("Product does not exists", response.ErrorMessage);
        Assert.Null(response.Data);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsInternalServerErrorResponse()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Throws(new Exception("Test exception"));

        var query = new GetProductByIdQuery(productId);

        // Act
        var response = await _handler.Handle(query, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.InternalServerError, response.Status);
        Assert.Equal("Something went wrong", response.ErrorMessage);
        Assert.Null(response.Data);
    }
}