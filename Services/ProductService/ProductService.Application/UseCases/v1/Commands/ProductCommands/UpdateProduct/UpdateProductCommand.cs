using MediatR;
using ProductService.Application.Models.Request.Products;
using ProductService.Application.Models.Response.Products;

namespace ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProduct;

public class UpdateProductCommand : IRequest<UpdateProductResponse>
{
    public Guid Id { get; set; }
    public UpdateProductRequest Payload { get; set; }

    public UpdateProductCommand(Guid id, UpdateProductRequest payload)
    {
        Id = id;
        Payload = payload;
    }
}