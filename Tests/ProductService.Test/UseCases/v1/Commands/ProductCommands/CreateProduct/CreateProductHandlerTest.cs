using AutoFixture;
using Microsoft.Extensions.Logging;
using MockQueryable.EntityFrameworkCore;
using Moq;
using ProductService.Application.Models.Request.Products;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.CreateProduct;
using ProductService.Domain.Abstraction;
using ProductService.Domain.Entities;
using Shared.Models.Response;
using Xunit;
using Assert = Xunit.Assert;

namespace ProductService.Test.UseCases.v1.Commands.ProductCommands.CreateProduct;

public class CreateProductHandlerTest
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateProductHandler _handler;
    private readonly Mock<ILogger<CreateProductHandler>> _logger;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken;

    public CreateProductHandlerTest()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger<CreateProductHandler>>();
        _cancellationToken = new CancellationToken();
        _fixture = new Fixture();
        _handler = new CreateProductHandler(_logger.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ProductAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        var productName = _fixture.Create<string>();
        var request = _fixture.Build<CreateProductRequest>()
            .With(x => x.Name, productName)
            .Create();

        var command = new CreateProductCommand(request);

        var existingProduct = _fixture.Build<Product>()
            .With(x => x.Name, productName)
            .With(x => x.IsDeleted, false)
            .Create();

        var products = new List<Product> { existingProduct }
            .AsQueryable()
            .BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(products);

        // Act
        var response = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.BadRequest, response.Status);
        Assert.NotNull(response.ErrorMessage);
        Assert.Equal("Product already exists", response.ErrorMessage);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsOkResponse()
    {
        // Arrange
        var productName = _fixture.Create<string>();
        var request = _fixture.Build<CreateProductRequest>()
            .With(x => x.Name, productName)
            .With(x => x.Quantity, _fixture.Create<int>())
            .With(x => x.Price, _fixture.Create<decimal>())
            .Create();

        var command = new CreateProductCommand(request);

        var products = new List<Product>()
            .AsQueryable()
            .BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(products);

        _unitOfWork.Setup(x => x.Product.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product product, CancellationToken token) => product);

        _unitOfWork.Setup(x => x.SaveChangesAsync(_cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.OK, response.Status);
        Assert.Empty(response.ErrorMessage);
    }

    [Fact]
    public async Task Handle_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var productName = _fixture.Create<string>();
        var request = _fixture.Build<CreateProductRequest>()
            .With(x => x.Name, productName)
            .Create();

        var command = new CreateProductCommand(request);

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Throws(new Exception("Database connection lost"));

        // Act
        var response = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.InternalServerError, response.Status);
        Assert.NotNull(response.ErrorMessage);
        Assert.Equal("Some error has occurred!", response.ErrorMessage);
    }

    [Fact]
    public async Task Handle_DatabaseSaveFailure_ReturnsInternalServerError()
    {
        // Arrange
        var productName = _fixture.Create<string>();
        var request = _fixture.Build<CreateProductRequest>()
            .With(x => x.Name, productName)
            .With(x => x.Quantity, _fixture.Create<int>())
            .With(x => x.Price, _fixture.Create<decimal>())
            .Create();

        var command = new CreateProductCommand(request);

        var products = new List<Product>()
            .AsQueryable()
            .BuildMock();

        _unitOfWork.Setup(x => x.Product.GetQueryable())
            .Returns(products);

        _unitOfWork.Setup(x => x.Product.AddAsync(It.IsAny<Product>(), _cancellationToken))
            .Throws(new Exception("Database save error"));

        // Act
        var response = await _handler.Handle(command, _cancellationToken);

        // Assert
        Assert.Equal((int)ResponseStatusCode.InternalServerError, response.Status);
        Assert.NotNull(response.ErrorMessage);
        Assert.Equal("Some error has occurred!", response.ErrorMessage);
    }
}