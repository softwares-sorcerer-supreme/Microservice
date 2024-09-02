using CartService.Application.Services.GrpcService;
using CartService.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Grpc.Protos;

namespace CartService.Infrastructure.StartupRegistration;

public static class GrpcRegistration
{
    public static IServiceCollection AddGrpcConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var grpcOptions = new GrpcOptions();
        configuration.GetSection(GrpcOptions.OptionName).Bind(grpcOptions);

        services.AddScoped<IProductService, Application.Services.GrpcService.ProductService>();
        services.AddGrpcClient<ProductProtoService.ProductProtoServiceClient>(
            o => o.Address = new Uri(grpcOptions.ProductServiceUrl));

        return services;
    }
}