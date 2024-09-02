using System.Reflection;
using CartService.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Application.StartupRegistration;

public static class CustomMediatorRegistration
{
    public static IServiceCollection AddCustomMediator(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        return services;
    }
}