using AutoFixture;
using CartService.Application.Models.Request.CartItems;
using CartService.Application.Services.GrpcService;
using CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart;
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

namespace CartService.Test.UseCases.v1.Commands.CartItemCommands.AddItemToCart;

public class AddItemToCartHandlerTest
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ILogger<AddItemToCartHandler>> _mockLogger;
    private readonly AddItemToCartHandler _handler;
    private readonly IFixture _fixture;
    
    public AddItemToCartHandlerTest()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<AddItemToCartHandler>>();
        _handler = new AddItemToCartHandler(_mockLogger.Object, _mockUnitOfWork.Object, _mockProductService.Object );
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Handle_CartDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var request = _fixture.Build<AddItemToCartCommand>()
            .With(x => x.Payload, _fixture.Create<AddItemToCartRequest>())
            .Create();
        
        _mockUnitOfWork.Setup(uow => uow.Cart.GetQueryable())
            .Returns(new List<Cart>().AsQueryable().BuildMock());

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal((int)ResponseStatusCode.BadRequest, response.Status);
        Assert.Equal(ResponseErrorMessageCode.ERR_CART_0001, response.ErrorMessageCode);
    }

    [Fact]
    public async Task Handle_ProductUpdateFails_ReturnsErrorResponse()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var payloadRequest = new AddItemToCartRequest
        {
            ProductId = Guid.NewGuid(),
            Quantity = 1
        };
        
        var request = _fixture.Build<AddItemToCartCommand>()
            .With(x => x.CartId, cartId)
            .With(x => x.Payload, payloadRequest)
            .Create();

        var cart = _fixture.Create<Cart>();
        cart.Id = cartId;
        
        var productResponse = new ProductModelResponse
        {
            Status = ResponseStatusCode.BadRequest.ToInt(),
            ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0002
        };

        // Mock the CartItem repository
        var cartItem = _fixture.Create<CartItem>();
        var cartItems = new List<CartItem> { cartItem }.AsQueryable().BuildMock();

        _mockUnitOfWork.Setup(uow => uow.Cart.GetQueryable())
            .Returns(new List<Cart> { cart }.AsQueryable().BuildMock());

        _mockUnitOfWork.Setup(uow => uow.CartItem.GetQueryable())
            .Returns(cartItems);

        _mockProductService.Setup(ps => ps.UpdateProductQuantity(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(productResponse);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal((int)ResponseStatusCode.BadRequest, response.Status);
        Assert.Equal(ResponseErrorMessageCode.ERR_SYS_0002, response.ErrorMessageCode);

        // Verify the interaction with CartItem
        _mockUnitOfWork.Verify(uow => uow.CartItem.UpdateAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.CartItem.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SuccessfulUpdate_ReturnsOkResponse()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var payloadRequest = new AddItemToCartRequest
        {
            ProductId = Guid.NewGuid(),
            Quantity = 1
        };
        
        var request = _fixture.Build<AddItemToCartCommand>()
            .With(x => x.CartId, cartId)
            .With(x => x.Payload, payloadRequest)
            .Create();
        var cart = _fixture.Create<Cart>();
        cart.Id = cartId;
        
        var productResponse = new ProductModelResponse
        {
            Status = ResponseStatusCode.OK.ToInt(),
            Product = new ProductModel { Price = "10.00" }
        };
        var cartItem = new CartItem
        {
            CartId = cartId,
            ProductId = request.Payload.ProductId,
            Quantity = request.Payload.Quantity
        };

        _mockUnitOfWork.Setup(uow => uow.Cart.GetQueryable())
            .Returns(new List<Cart> { cart }.AsQueryable().BuildMock());
        _mockUnitOfWork.Setup(uow => uow.CartItem.GetQueryable())
            .Returns(new List<CartItem>().AsQueryable().BuildMock());
        _mockProductService.Setup(ps => ps.UpdateProductQuantity(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(productResponse);
        _mockUnitOfWork.Setup(uow => uow.CartItem.AddAsync(It.IsAny<CartItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cartItem); // Return the entity added
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal((int)ResponseStatusCode.OK, response.Status);
        Assert.Empty(response.ErrorMessageCode);
    }
}