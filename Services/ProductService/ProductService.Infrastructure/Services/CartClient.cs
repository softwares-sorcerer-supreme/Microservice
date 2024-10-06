using Microsoft.Extensions.Logging;
using ProductService.Application.Services;
using Shared.HttpClientCustom;

namespace ProductService.Infrastructure.Services;

public class CartClient : ICartClient
{
    private readonly IHttpClientCustom<CartServiceClient> _cartServiceClient;
    private readonly ILogger<CartClient> _logger;

    public CartClient
    (
        IHttpClientCustom<CartServiceClient> cartServiceClient,
        ILogger<CartClient> logger
    )
    {
        _cartServiceClient = cartServiceClient;
        _logger = logger;
    }

    public async Task HealthCheckCartService(CancellationToken cancellationToken)
    {
        const string funcName = $"{nameof(CartClient)} {nameof(HealthCheckCartService)}";
        _logger.LogInformation(funcName);

        try
        {
            var response = await _cartServiceClient.GetAsync<object>("/api/v1/Cart/health-check", cancellationToken);
            if (response == null)
            {
                _logger.LogError($"{funcName} has error: response is null");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{funcName} has error: {ex.Message}");
        }
    }
}