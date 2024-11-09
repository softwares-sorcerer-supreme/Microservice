using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AuthService.Application.StartupRegistration;

public static class FluentValidationRegistration
{
    public static IServiceCollection AddValidatorConfiguration(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}