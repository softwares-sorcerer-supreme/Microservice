using MediatR;
using ProductService.Application.Models.Response.Products;
using Shared.Models.Request;

namespace ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProducts;

public class GetProductsQuery : IRequest<GetProductsResponse>
{
    public PagingRequest Payload { get; set; }

    public GetProductsQuery(PagingRequest payload)
    {
        Payload = payload;
    }
}