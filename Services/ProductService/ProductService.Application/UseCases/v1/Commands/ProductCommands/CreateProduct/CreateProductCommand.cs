using MediatR;
using ProductService.Application.Models.Request.Products;
using ProductService.Application.Models.Response.Categories;

namespace ProductService.Application.UseCases.v1.Commands.ProductCommands.CreateProduct;

public class CreateProductCommand : IRequest<CreateProductResponse>
{
    public CreateProductRequest Payload { get; set; }
    public CreateProductCommand(CreateProductRequest payload)
    {
        Payload = payload;
    }
}
