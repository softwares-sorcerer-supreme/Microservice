using Asp.Versioning;
using CartService.Application.Models.Request.CartItems;
using CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart;
using CartService.Application.UseCases.v1.Commands.CartItemCommands.RemoveItemFromCart;
using CartService.Application.UseCases.v1.Queries.CartItemQueries.GetItemsByCartId;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Response;

namespace CartService.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/{api_version:apiVersion}/[controller]")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    public CartController(IMediator mediator)
    {
        _mediator = mediator;
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
}