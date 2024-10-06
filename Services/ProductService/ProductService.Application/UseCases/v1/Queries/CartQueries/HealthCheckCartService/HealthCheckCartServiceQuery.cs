using MediatR;
using ProductService.Application.Models.Response.Carts;

namespace ProductService.Application.UseCases.v1.Queries.CartQueries.HealthCheckCartService;

public class HealthCheckCartServiceQuery : IRequest<HealthCheckCartServiceResponse>;