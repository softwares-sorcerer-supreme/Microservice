using Asp.Versioning;

namespace CartService.API.StartupRegistration;

public static class ApiVersioningRegistration
{
    public static IServiceCollection AddConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(x =>
        {
            x.DefaultApiVersion = new ApiVersion(1, 0);
            x.AssumeDefaultVersionWhenUnspecified = true;
            x.ReportApiVersions = true;
            x.UnsupportedApiVersionStatusCode = StatusCodes.Status400BadRequest;
        });

        return services;
    }
}