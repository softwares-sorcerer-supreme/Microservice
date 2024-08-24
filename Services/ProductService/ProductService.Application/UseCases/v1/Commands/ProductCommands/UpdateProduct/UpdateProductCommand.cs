using MediatR;
using ProductService.Application.Models.Request.Products;
using ProductService.Application.Models.Response.Categories;

namespace ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProduct;

public class UpdateProductCommand : IRequest<UpdateCategoryResponse>
{
    public UpdateProductRequest Payload { get; set; }
    
    public UpdateProductCommand(UpdateProductRequest payload)
    {
        Payload = payload;
    }
}
