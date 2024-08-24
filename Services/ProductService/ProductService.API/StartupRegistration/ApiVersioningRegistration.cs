using Asp.Versioning;

namespace ProductService.API.StartupRegistration;

public static class ApiVersioningRegistration
{
    public static IServiceCollection AddConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(x =>
        {
            x.DefaultApiVersion = new ApiVersion(1, 0);
            x.AssumeDefaultVersionWhenUnspecified = true;
            x.ReportApiVersions = true;
        });

        return services;
    }
}