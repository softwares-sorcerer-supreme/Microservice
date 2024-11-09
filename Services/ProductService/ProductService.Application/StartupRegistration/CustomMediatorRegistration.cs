using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Behaviors;
using System.Reflection;

namespace ProductService.Application.StartupRegistration;

public static class CustomMediatorRegistration
{
    public static IServiceCollection AddCustomMediator(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        return services;
    }
}