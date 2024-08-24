using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Grpc.Protos;

namespace CartService.Application.StartupRegistration;

public static class GrpcRegistration
{
    public static IServiceCollection AddGrpcConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<Services.GrpcService.ProductService>();
        services.AddGrpcClient<ProductProtoService.ProductProtoServiceClient>(
            o => o.Address = new Uri(configuration.GetValue<string>("GrpcSettings:ProductServiceUrl") ?? string.Empty));

        return services;
    }
}