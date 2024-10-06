namespace ProductService.Application.Services;

public interface ICartClient
{
    Task HealthCheckCartService(CancellationToken cancellationToken);
}