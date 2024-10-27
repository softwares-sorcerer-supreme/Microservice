using CartService.Application.Abstractions.Services.EventMessageService;
using CartService.Application.Services.GrpcService;
using CartService.Infrastructure.Options;
using CartService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Grpc.Protos;

namespace CartService.Infrastructure.StartupRegistration;

public static class ServiceRegistration
{
    public static IServiceCollection AddServicesConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        //gRPC Config
        var grpcOptions = new GrpcOptions();
        configuration.GetSection(GrpcOptions.OptionName).Bind(grpcOptions);

        services.AddScoped<IProductService, Services.ProductService>();
        services.AddGrpcClient<ProductProtoService.ProductProtoServiceClient>(o => o.Address = new Uri(grpcOptions.ProductServiceUrl));

        //Event message
        services.AddScoped<ISendMessageService, SendMessageService>();
        
        return services;
    }

}