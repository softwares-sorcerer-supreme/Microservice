using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Models.Response.Carts;
using ProductService.Application.Services;
using Shared.CommonExtension;
using Shared.Constants;
using Shared.Enums;

namespace ProductService.Application.UseCases.v1.Queries.CartQueries.HealthCheckCartService;

public class HealthCheckCartServiceHandler : IRequestHandler<HealthCheckCartServiceQuery, HealthCheckCartServiceResponse>
{
    private readonly ICartClient _cartClient;
    private readonly ILogger<HealthCheckCartServiceHandler> _logger;

    public HealthCheckCartServiceHandler
    (
        ICartClient cartClient,
        ILogger<HealthCheckCartServiceHandler> logger
    )
    {
        _cartClient = cartClient;
        _logger = logger;
    }
    
    public async Task<HealthCheckCartServiceResponse> Handle(HealthCheckCartServiceQuery request, CancellationToken cancellationToken)
    {
        const string funcName = nameof(HealthCheckCartServiceHandler);
        _logger.LogInformation(funcName);
        var response = new HealthCheckCartServiceResponse
        {
            Status = ResponseStatusCode.OK.ToInt()
        };
        
        try
        {
            await _cartClient.HealthCheckCartService(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{funcName} has error: {ex.Message}");
            response.Status = ResponseStatusCode.InternalServerError.ToInt();
            response.ErrorMessageCode = ResponseErrorMessageCode.ERR_SYS_0001;
        }

        return response;
    }
}