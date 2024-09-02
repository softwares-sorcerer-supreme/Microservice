using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ProductService.Infrastructure.StartupRegistration;

public static class GrpcRegistration
{
    public static IServiceCollection AddGrpcConfiguration(this IServiceCollection services)
    {
        services.AddGrpc();
        return services;
    }
    
    public static WebApplication MapGrpcConfiguration(this WebApplication app)
    {
        app.MapGrpcService<Application.Services.ProductService>();
        return app;
    }
}