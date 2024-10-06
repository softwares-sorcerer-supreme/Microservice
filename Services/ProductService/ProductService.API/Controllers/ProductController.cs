using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Models.Request.Products;
using ProductService.Application.Models.Response.Products;
using ProductService.Application.Services;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.CreateProduct;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.DeleteProduct;
using ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProduct;
using ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductById;
using ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProducts;
using Shared.Models.Request;
using Shared.Models.Response;

namespace ProductService.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{api_version:apiVersion}/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICartClient _cartClient;
    public ProductController(IMediator mediator, ICartClient cartClient)
    {
        _mediator = mediator;
        _cartClient = cartClient;
    }
    
    [HttpGet]
    [Route("test")]
    public async Task<IActionResult> Test(CancellationToken cancellationToken)
    {
        await _cartClient.HealthCheckCartService(cancellationToken);
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] PagingRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetProductsQuery(request), cancellationToken);
        return ResponseHelper.ToPaginationResponse(response.Status, response.ErrorMessageCode, response.Data, response.Paging);
    }
    
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return ResponseHelper.ToPaginationResponse(response.Status, response.ErrorMessageCode, response.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest productRequest, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateProductCommand(productRequest), cancellationToken);
        return ResponseHelper.ToResponse(response.Status, response.ErrorMessageCode);
    }
     
    [HttpPut]
    [Route("{id}")]
    // [ProducesResponseType(StatusCode = StatusCodes.Status200OK, Type = typeof(UpdateProductResponse))]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new UpdateProductCommand(id, request), cancellationToken);
        return ResponseHelper.ToResponse(response.Status, response.ErrorMessageCode);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.Status, response.ErrorMessageCode);
    }
    
    
    
}