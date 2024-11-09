using MediatR;
using ProductService.Application.Models.Response.Products;

namespace ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductByIdDapper;

public class GetProductByIdDapperQuery : IRequest<GetProductByIdResponse>
{
    public Guid Id { get; set; }

    public GetProductByIdDapperQuery(Guid id)
    {
        Id = id;
    }
}