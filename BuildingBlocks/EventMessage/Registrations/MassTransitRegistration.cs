using System.Net.Mime;
using System.Reflection;
using EventMessage.Core;
using EventMessage.Options;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Shared.Extensions;

namespace EventMessage.Registrations;

public static class MassTransitRegistration
{
    public static IServiceCollection AddMassTransitConfiguration(this IServiceCollection services, Assembly? entryAssembly = null)
    {
        var currentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var rabbitMqOption = services.GetOptions<RabbitMqOption>(RabbitMqOption.OptionName);
        
        services.AddMassTransit(busConfigurator =>
        {
            if(entryAssembly is not null)
            {
                busConfigurator.AddConsumers(entryAssembly);
            }
            
            busConfigurator.SetKebabCaseEndpointNameFormatter();
            if (currentEnv is "Development")
            {
                busConfigurator.UsingRabbitMq((ctx,cfg) =>
                {
                    cfg.Host(rabbitMqOption.HostName, rabbitMqOption.Port, rabbitMqOption.VirtualHost, h => {
                        h.Username(rabbitMqOption.Username);
                        h.Password(rabbitMqOption.Password);
                    });
                    
                    cfg.ConfigureEndpoints(ctx);
                });
            }
        });

        services.AddScoped<IMessageSender, MessageSender>();
        return services;
    }
    
    public static WebApplication UseMassTransitHealthCheck(this WebApplication app)
    {
        app.MapHealthChecks("/health/ready", new HealthCheckOptions()
        {
            Predicate = (check) => check.Tags.Contains("ready"),
        });

        app.MapHealthChecks("/health/live", new HealthCheckOptions());
        
        return app;
    }
    
}