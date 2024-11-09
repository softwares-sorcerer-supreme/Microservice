using AutoFixture;
using Microsoft.Extensions.Logging;
using MockQueryable.EntityFrameworkCore;
using Moq;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.DeleteProduct;
using ProductService.Domain.Abstraction;
using ProductService.Domain.Entities;
using Shared.Enums;
using Xunit;
using Assert = Xunit.Assert;

namespace ProductService.Test.UseCases.v1.Commands.ProductCommands.DeleteProduct;

public class DeleteProductHandlerTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly DeleteProductHandler _handler;
    private readonly Mock<ILogger<DeleteProductHandler>> _logger;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public DeleteProductHandlerTest()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger<DeleteProductHandler>>();
        _cancellationToken = new CancellationToken();
        _fixture = new Fixture();
        _handler = new DeleteProductHandler(_logger.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ProductExists_ReturnsOkResponse()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();
        var command = new DeleteProductCommand(productId);

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
    public async Task Handle_ProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();
        var command = new DeleteProductCommand(productId);

        var products = new List<Product>()
            .AsQueryable()
            .BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(products);

        // Act
        var response = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.NotFound, response.Status);
        Assert.Equal("Product does not exists", response.ErrorMessageCode);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();
        var command = new DeleteProductCommand(productId);

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Throws(new Exception("Database connection lost"));

        // Act
        var response = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.InternalServerError, response.Status);
        Assert.Equal("DeleteProductHandler => Some error has occured!", response.ErrorMessageCode);
    }
}