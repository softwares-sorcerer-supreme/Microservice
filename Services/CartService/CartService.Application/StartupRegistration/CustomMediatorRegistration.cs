using System.Reflection;
using CartService.Application.Behaviors;
using CartService.Application.UseCases.v1.Commands.CartItemCommands.AddItemToCart.PostProcessor;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Application.StartupRegistration;

public static class CustomMediatorRegistration
{
    public static IServiceCollection AddCustomMediator(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            cfg.AddRequestPostProcessor(typeof(AddItemToCartPostProcessor));
            cfg.AutoRegisterRequestProcessors = false;
        });
        
        // services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        return services;
    }
}