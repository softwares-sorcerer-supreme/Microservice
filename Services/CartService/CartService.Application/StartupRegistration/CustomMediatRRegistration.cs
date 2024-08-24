using System.Reflection;
using CartService.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Application.StartupRegistration;

public static class CustomMediatRRegistration
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        return services;
    }
}