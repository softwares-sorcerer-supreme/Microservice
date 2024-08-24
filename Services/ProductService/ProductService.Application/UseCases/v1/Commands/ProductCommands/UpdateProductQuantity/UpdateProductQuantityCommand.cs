using MediatR;
using ProductService.Application.Grpc.Protos;
using ProductService.Application.Models.Response.Products;

namespace ProductService.Application.UseCases.v1.Commands.ProductCommands.UpdateProductQuantity;

public class UpdateProductQuantityCommand : IRequest<UpdateProductQuantityResponse>
{
    public UpdateProductQuantityRequest Payload { get; set; }
    
    public UpdateProductQuantityCommand(UpdateProductQuantityRequest payload)
    {
        Payload = payload;
    }
}
