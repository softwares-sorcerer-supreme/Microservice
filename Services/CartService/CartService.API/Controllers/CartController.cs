using Asp.Versioning;
using CartService.Application.Abstractions.Services.EventMessageService;
using CartService.Application.Models.Request.CartItems;
using CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart;
using CartService.Application.UseCases.v1.Commands.CartItemCommands.RemoveItemFromCart;
using CartService.Application.UseCases.v1.Queries.CartItemQueries.GetItemsByCartId;
using CartService.Domain.Abstraction;
using CartService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Response;

namespace CartService.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{api_version:apiVersion}/[controller]")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    //Only for test
    private readonly IUnitOfWorkMongoDb _unitOfWorkMongoDb;

    private readonly ISendMessageService _sendMessageService;

    public CartController
    (
        IMediator mediator,
        IUnitOfWorkMongoDb unitOfWorkMongoDb,
        ISendMessageService sendMessageService
    )
    {
        _mediator = mediator;
        _unitOfWorkMongoDb = unitOfWorkMongoDb;
        _sendMessageService = sendMessageService;
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
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> AddItemToCart(Guid id, [FromBody] AddItemToCartRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new AddItemToCartCommand(id, request), cancellationToken);
        return ResponseHelper.ToPaginationResponse(response.Status, response.ErrorMessageCode);
    }

    [HttpDelete]
    [Route("{id}/products/{productId}")]
    [Authorize(Policy = "AdminPolicy")]
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
    public async Task<IActionResult> HealthCheck(CancellationToken cancellationToken)
    {
        throw new Exception("Test Exception");
    }

    [HttpGet]
    [Route("load-balancer")]
    public async Task<IActionResult> LoadBalancer(CancellationToken cancellationToken)
    {
        return Ok($"Address: {Request.GetDisplayUrl()}");
    }

    [HttpPost]
    [Route("test-insert-mongo")]
    public async Task<IActionResult> TestMongo(Cart cart, CancellationToken cancellationToken)
    {
        await _unitOfWorkMongoDb.Cart.AddAsync(cart, cancellationToken);
        await _unitOfWorkMongoDb.SaveChangesAsync(cancellationToken);
        return Ok("Ok");
    }

    [HttpPost]
    [Route("test-mongo-search")]
    public async Task<IActionResult> TestMongo(Guid id, CancellationToken cancellationToken)
    {
        var a = _unitOfWorkMongoDb.Cart.GetQueryable();
        var result = await a.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("test-exception")]
    public async Task<IActionResult> TestExceptional(CancellationToken cancellationToken)
    {
        throw new Exception("Test Exception");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="type">Send = 1, Publish = 2</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("test-event-message")]
    public async Task<IActionResult> TestEventMessage(SendMessageType type, CancellationToken cancellationToken)
    {
        if (type == SendMessageType.Publish)
        {
            await _sendMessageService.PublishAddToCartNotification(cancellationToken);
        }
        else
        {
            await _sendMessageService.SendAddToCartNotification(cancellationToken);
        }

        return Ok("Ok");
    }
}

public enum SendMessageType
{
    Send = 1,
    Publish
}