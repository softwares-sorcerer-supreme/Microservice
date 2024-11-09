using AutoFixture;
using Microsoft.Extensions.Logging;
using MockQueryable.EntityFrameworkCore;
using Moq;
using ProductService.Application.Models.Request.Products;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProductQuantity;
using ProductService.Domain.Abstraction;
using ProductService.Domain.Entities;
using Shared.Enums;
using Xunit;
using Assert = Xunit.Assert;

namespace ProductService.Test.UseCases.v1.Commands.ProductCommands.UpdateProductQuantity;

public class UpdateProductQuantityHandlerTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UpdateProductQuantityHandler _handler;
    private readonly Mock<ILogger<UpdateProductQuantityHandler>> _logger;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public UpdateProductQuantityHandlerTest()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger<UpdateProductQuantityHandler>>();
        _cancellationToken = new CancellationToken();
        _fixture = new Fixture();
        _handler = new UpdateProductQuantityHandler(_logger.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ProductExistsAndQuantitySufficient_ReturnsOkResponse()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();

        var quantityIssued = 10;
        var updateRequest = _fixture.Build<UpdateProductQuantityRequest>()
            .With(x => x.Id, productId)
            .With(x => x.Quantity, quantityIssued)
            .Create();

        var command = new UpdateProductQuantityCommand(updateRequest);

        var quantityLeft = 100;
        var existingProduct = _fixture.Build<Product>()
            .With(x => x.Id, productId)
            .With(x => x.IsDeleted, false)
            .With(x => x.Quantity, quantityLeft) // Ensure sufficient quantity
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
        Assert.Empty(response.ErrorMessageCode);  // Check that there is no error message
        Assert.NotNull(response.Data);
        Assert.Equal(existingProduct.Id, response.Data.Id);
        Assert.Equal(existingProduct.Name, response.Data.Name);
        Assert.Equal(existingProduct.Quantity, quantityLeft - quantityIssued);
        Assert.Equal(existingProduct.Price, response.Data.Price);
    }

    [Fact]
    public async Task Handle_ProductDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();
        var updateRequest = _fixture.Build<UpdateProductQuantityRequest>()
            .With(x => x.Id, productId)
            .Create();

        var products = new List<Product>()
            .AsQueryable()
            .BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(products);

        // Act
        var response = await _handler.Handle(new UpdateProductQuantityCommand(updateRequest), _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.BadRequest, response.Status);
        Assert.Equal("Product does not exists", response.ErrorMessageCode);
    }

    [Fact]
    public async Task Handle_InsufficientQuantity_ReturnsBadRequest()
    {
        // Arrange
        var productId = _fixture.Create<Guid>();
        var updateRequest = _fixture.Build<UpdateProductQuantityRequest>()
            .With(x => x.Id, productId)
            .With(x => x.Quantity, 20) // Requesting more than available
            .Create();

        var existingProduct = _fixture.Build<Product>()
            .With(x => x.Id, productId)
            .With(x => x.IsDeleted, false)
            .With(x => x.Quantity, 10) // Insufficient quantity
            .Create();

        var products = new List<Product> { existingProduct }
            .AsQueryable()
            .BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(products);

        // Act
        var response = await _handler.Handle(new UpdateProductQuantityCommand(updateRequest), _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.BadRequest, response.Status);
        Assert.Equal("Product quantity is not enough", response.ErrorMessageCode);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var updateRequest = _fixture.Build<UpdateProductQuantityRequest>().Create();
        var command = new UpdateProductQuantityCommand(updateRequest);

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Throws(new Exception("Database connection lost"));

        // Act
        var response = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.InternalServerError, response.Status);
        Assert.Equal("UpdateProductQuantityHandler => ProductId = " + updateRequest.Id + " => Some error has occurred!", response.ErrorMessageCode);
    }
}