﻿using AuthService.Application.Behaviors;
using AuthService.Application.UseCases.v1.Commands.Login;
using AuthService.Application.UseCases.v1.Commands.RenewToken;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Caching.Behaviors;

namespace AuthService.Application.StartupRegistration;

public static class CustomMediatorRegistration
{
    public static IServiceCollection AddMediatorConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddRequestPostProcessor<LoginPostProcessor>();
            cfg.AddRequestPostProcessor<RenewTokenPostProcessor>();
            cfg.AutoRegisterRequestProcessors = true;
        });
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>));
        return services;
    }
}