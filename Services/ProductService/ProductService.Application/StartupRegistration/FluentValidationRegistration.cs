using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ProductService.Application.StartupRegistration;

public static class FluentValidationRegistration
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}