using MediatR;
using ProductService.Application.Models.Response.Products;

namespace ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductById;

public class GetProductByIdQuery : IRequest<GetProductByIdResponse>
{
    public Guid Id { get; set; }

    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }
}