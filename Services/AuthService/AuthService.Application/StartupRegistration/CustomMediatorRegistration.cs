using System.Reflection;
using AuthService.Application.Behaviors;
using AuthService.Application.UseCases.v1.Commands.Login;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Application.StartupRegistration;

public static class CustomMediatorRegistration
{
    public static IServiceCollection AddMediatorConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddRequestPostProcessor<LoginPostProcessor>();
            cfg.AutoRegisterRequestProcessors = true;
        });
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        
        return services;
    }
}