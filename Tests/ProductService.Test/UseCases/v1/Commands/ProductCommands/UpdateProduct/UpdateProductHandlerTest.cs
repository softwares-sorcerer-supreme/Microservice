using AutoFixture;
using Microsoft.Extensions.Logging;
using MockQueryable.EntityFrameworkCore;
using Moq;
using ProductService.Application.Models.Request.Products;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProduct;
using ProductService.Domain.Abstraction;
using ProductService.Domain.Entities;
using Shared.Enums;
using Shared.Models.Response;
using Xunit;
using Assert = Xunit.Assert;

namespace ProductService.Test.UseCases.v1.Commands.ProductCommands.UpdateProduct;

public class UpdateProductHandlerTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UpdateProductHandler _handler;
    private readonly Mock<ILogger<UpdateProductHandler>> _logger;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public UpdateProductHandlerTest()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger<UpdateProductHandler>>();
        _cancellationToken = new CancellationToken();
        _fixture = new Fixture();
        _handler = new UpdateProductHandler(_logger.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ProductExists_ReturnsOkResponse()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();
        var updateRequest = _fixture.Build<UpdateProductRequest>()
            .With(x => x.Name, _fixture.Create<string>())
            .With(x => x.Quantity, _fixture.Create<int>())
            .With(x => x.Price, _fixture.Create<decimal>())
            .Create();

        var command = new UpdateProductCommand(productId, updateRequest);

        var existingProduct = _fixture.Build<Product>()
            .With(x => x.Id, productId)
            .With(x => x.IsDeleted, false)
            .Create();

        var products = new List<Product> { existingProduct }
            .AsQueryable()
            .BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(products);

        _unitOfWork.Setup(x => x.Product.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product product, CancellationToken token) => product);

        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.OK, response.Status);
        Assert.Empty(response.ErrorMessageCode); // Check that there is no error message
    }

    [Fact]
    public async Task Handle_ProductDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();
        var updateRequest = _fixture.Build<UpdateProductRequest>()
            .Create();

        var command = new UpdateProductCommand(productId, updateRequest);

        var products = new List<Product>()
            .AsQueryable()
            .BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(products);

        // Act
        var response = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.BadRequest, response.Status);
        Assert.Equal("Product does not exists", response.ErrorMessageCode);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();
        var updateRequest = _fixture.Build<UpdateProductRequest>()
            .Create();

        var command = new UpdateProductCommand(productId, updateRequest);

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Throws(new Exception("Database connection lost"));

        // Act
        var response = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.InternalServerError, response.Status);
        Assert.Equal("CreateProductHandler Handler => Some error has occurred!", response.ErrorMessageCode);
    }
}