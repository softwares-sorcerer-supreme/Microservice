using AutoFixture;
using CartService.Application.Models.Request.CartItems;
using CartService.Application.Services.GrpcService;
using CartService.Application.UseCases.v1.Commands.CartItemCommands.RemoveItemFromCart;
using CartService.Domain.Abstraction;
using CartService.Domain.Entities;
using Microsoft.Extensions.Logging;
using MockQueryable.EntityFrameworkCore;
using Moq;
using ProductService.Application.Grpc.Protos;
using Shared.CommonExtension;
using Shared.Constants;
using Shared.Enums;
using Shared.Models.Response;

namespace CartService.Test.UseCases.v1.Commands.CartItemCommands.RemoveItemFromCart;

public class RemoveItemFromCartHandlerTest
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ILogger<RemoveItemFromCartHandler>> _mockLogger;
    private readonly RemoveItemFromCartHandler _handlerTest;
    private readonly Fixture _fixture;

    public RemoveItemFromCartHandlerTest()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<RemoveItemFromCartHandler>>();
        _handlerTest = new RemoveItemFromCartHandler(
            _mockLogger.Object,
            _mockUnitOfWork.Object, 
            _mockProductService.Object
            );
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Handle_CartDoesNotExist_ReturnsErrorResponse()
    {
        // Arrange
        var request = _fixture.Build<RemoveItemFromCartCommand>()
            .With(x => x.Payload, _fixture.Create<RemoveItemFromCartRequest>())
            .Create();

        _mockUnitOfWork.Setup(uow => uow.Cart.GetQueryable())
            .Returns(new List<Cart>().AsQueryable().BuildMock());

        // Act
        var response = await _handlerTest.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal((int)ResponseStatusCode.BadRequest, response.Status);
        Assert.Equal(ResponseErrorMessageCode.ERR_SYS_0002, response.ErrorMessageCode);
    }

    [Fact]
    public async Task Handle_CartItemDoesNotExist_ReturnsErrorResponse()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var payloadRequest = new RemoveItemFromCartRequest
        {
            CartId = cartId,
            ProductId = Guid.NewGuid(),
        };
        
        var request = _fixture.Build<RemoveItemFromCartCommand>()
            .With(x => x.Payload, payloadRequest)
            .Create();
        var cart = _fixture.Create<Cart>();
        cart.Id = cartId;
        
        _mockUnitOfWork.Setup(uow => uow.Cart.GetQueryable())
            .Returns(new List<Cart> { cart }.AsQueryable().BuildMock());

        _mockUnitOfWork.Setup(uow => uow.CartItem.GetQueryable())
            .Returns(new List<CartItem>().AsQueryable().BuildMock());

        // Act
        var response = await _handlerTest.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal((int)ResponseStatusCode.BadRequest, response.Status);
        Assert.Equal(ResponseErrorMessageCode.ERR_CART_0002, response.ErrorMessageCode);
    }

    [Fact]
    public async Task Handle_ProductUpdateFails_ReturnsErrorResponse()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var payloadRequest = new RemoveItemFromCartRequest
        {
            CartId = cartId,
            ProductId = Guid.NewGuid(),
        };
        
        var request = _fixture.Build<RemoveItemFromCartCommand>()
            .With(x => x.Payload, payloadRequest)
            .Create();
        var cart = _fixture.Create<Cart>();
        cart.Id = cartId;
        
        var cartItem = new CartItem
        {
            CartId = request.Payload.CartId,
            ProductId = request.Payload.ProductId,
            Quantity = 5 // Example quantity
        };
        var productResponse = new ProductModelResponse
        {
            Status = ResponseStatusCode.BadRequest.ToInt(),
            ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0002
        };

        _mockUnitOfWork.Setup(uow => uow.Cart.GetQueryable())
            .Returns(new List<Cart> { cart }.AsQueryable().BuildMock());

        _mockUnitOfWork.Setup(uow => uow.CartItem.GetQueryable())
            .Returns(new List<CartItem> { cartItem }.AsQueryable().BuildMock());

        _mockProductService.Setup(ps => ps.UpdateProductQuantity(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(productResponse);

        // Act
        var response = await _handlerTest.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal((int)ResponseStatusCode.BadRequest, response.Status);
        Assert.Equal(ResponseErrorMessageCode.ERR_SYS_0002, response.ErrorMessageCode);

        _mockUnitOfWork.Verify(uow => uow.CartItem.RemoveAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SuccessfulRemoval_UpdatesCartAndRemovesCartItem()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var payloadRequest = new RemoveItemFromCartRequest
        {
            CartId = cartId,
            ProductId = Guid.NewGuid(),
        };
        
        var request = _fixture.Build<RemoveItemFromCartCommand>()
            .With(x => x.Payload, payloadRequest)
            .Create();
        var cart = _fixture.Create<Cart>();
        cart.Id = cartId;
        
        var cartItem = new CartItem
        {
            CartId = request.Payload.CartId,
            ProductId = request.Payload.ProductId,
            Quantity = 5 // Example quantity
        };
        var productResponse = new ProductModelResponse
        {
            Status = ResponseStatusCode.OK.ToInt(),
            Product = new ProductModel { Price = "10.00" }
        };

        _mockUnitOfWork.Setup(uow => uow.Cart.GetQueryable())
            .Returns(new List<Cart> { cart }.AsQueryable().BuildMock());

        _mockUnitOfWork.Setup(uow => uow.CartItem.GetQueryable())
            .Returns(new List<CartItem> { cartItem }.AsQueryable().BuildMock());

        _mockProductService.Setup(ps => ps.UpdateProductQuantity(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(productResponse);

        // Act
        var response = await _handlerTest.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal((int)ResponseStatusCode.OK, response.Status);
        Assert.Empty(response.ErrorMessageCode);

        _mockUnitOfWork.Verify(uow => uow.CartItem.RemoveAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExceptionOccurred_ReturnsErrorResponse()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var payloadRequest = new RemoveItemFromCartRequest
        {
            CartId = cartId,
            ProductId = Guid.NewGuid(),
        };
        
        var request = _fixture.Build<RemoveItemFromCartCommand>()
            .With(x => x.Payload, payloadRequest)
            .Create();
        var cart = _fixture.Create<Cart>();
        cart.Id = cartId;
        
        var cartItem = new CartItem
        {
            CartId = request.Payload.CartId,
            ProductId = request.Payload.ProductId,
            Quantity = 5 // Example quantity
        };
        _mockUnitOfWork.Setup(uow => uow.Cart.GetQueryable())
            .Returns(new List<Cart> { cart }.AsQueryable().BuildMock());

        _mockUnitOfWork.Setup(uow => uow.CartItem.GetQueryable())
            .Returns(new List<CartItem> { cartItem }.AsQueryable().BuildMock());

        _mockProductService.Setup(ps => ps.UpdateProductQuantity(It.IsAny<Guid>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var response = await _handlerTest.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal((int)ResponseStatusCode.InternalServerError, response.Status);
        Assert.Contains(ResponseErrorMessageCode.ERR_SYS_0001, response.ErrorMessageCode);
    }
}