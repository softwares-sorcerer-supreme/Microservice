using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Behaviors;
using System.Reflection;
using Caching.Behaviors;

namespace ProductService.Application.StartupRegistration;

public static class CustomMediatorRegistration
{
    public static IServiceCollection AddCustomMediator(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>));
        return services;
    }
}