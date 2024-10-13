using Asp.Versioning;
using CartService.Application.Models.Request.CartItems;
using CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart;
using CartService.Application.UseCases.v1.Commands.CartItemCommands.RemoveItemFromCart;
using CartService.Application.UseCases.v1.Queries.CartItemQueries.GetItemsByCartId;
using CartService.Domain.Abstraction.Repositories.MongoDb;
using CartService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Response;

namespace CartService.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{api_version:apiVersion}/[controller]")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICartMongoRepository _mongoRepository;

    public CartController
    (
        IMediator mediator,
        ICartMongoRepository mongoRepository
    )
    {
        _mediator = mediator;
        _mongoRepository = mongoRepository;
    }

    [HttpGet]
    [Route("{id}/products")]
    
    public async Task<IActionResult> GetItemsByCartId(Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetItemsByCartIdQuery(id), cancellationToken);
        return ResponseHelper.ToPaginationResponse(response.Status, response.ErrorMessageCode,
            response.Data);
    }

    [HttpPost]
    [Route("{id}/products")]
    public async Task<IActionResult> AddItemToCart(Guid id, [FromBody] AddItemToCartRequest request,CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new AddItemToCartCommand(id, request), cancellationToken);
        return ResponseHelper.ToPaginationResponse(response.Status, response.ErrorMessageCode);
    }

    [HttpDelete]
    [Route("{id}/products/{productId}")]
    public async Task<IActionResult> RemoveItemFromCart(Guid id, Guid productId, CancellationToken cancellationToken)
    {
        var request = new RemoveItemFromCartRequest
        {
            CartId = id,
            ProductId = productId
        };
        var response = await _mediator.Send(new RemoveItemFromCartCommand(request), cancellationToken);
        return ResponseHelper.ToPaginationResponse(response.Status, response.ErrorMessageCode);
    }

    [HttpGet]
    [Route("health-check")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<IActionResult> HealthCheck(CancellationToken cancellationToken)
    {
        Console.WriteLine("health-check called: " + DateTime.Now.ToLongTimeString());
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        Console.WriteLine("health-check done: " + DateTime.Now.ToLongTimeString());
        return Ok("Ok");
    }

    [HttpPost]
    [Route("test-insert-mongo")]
    public async Task<IActionResult> TestMongo(Cart cart, CancellationToken cancellationToken)
    {
        await _mongoRepository.InsertOneAsync(cart, cancellationToken);
        return Ok("Ok");
    }
    

    [HttpPost]
    [Route("test-mongo-search")]
    public async Task<IActionResult> TestMongo(Guid id, CancellationToken cancellationToken)
    {
        var a = await _mongoRepository.FindOneAsync(filterExpression => filterExpression.Id == id, cancellationToken);
        return Ok(a);
    }
}