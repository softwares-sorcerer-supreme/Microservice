using MediatR;
using ProductService.Application.Models.Response.Products;

namespace ProductService.Application.UseCases.v1.Commands.ProductCommands.DeleteProduct;

public class DeleteProductCommand : IRequest<DeleteProductResponse>
{
    public Guid Id { get; set; }
    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }
}