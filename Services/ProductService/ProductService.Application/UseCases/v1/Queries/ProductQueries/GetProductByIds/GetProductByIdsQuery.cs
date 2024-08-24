using MediatR;
using ProductService.Application.Models.Response.Products;

namespace ProductService.Application.UseCases.v1.Queries.ProductQueries.GetProductByIds;

public class GetProductByIdsQuery : IRequest<GetProductsByIdsResponse>
{
    public List<Guid> Ids { get; set; }
    public GetProductByIdsQuery(List<Guid> ids)
    {
        Ids = ids;
    }
}