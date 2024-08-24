using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CartService.Application.StartupRegistration;

public static class FluentValidationRegistration
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}