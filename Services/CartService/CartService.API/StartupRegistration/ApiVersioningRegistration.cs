using Asp.Versioning;

namespace CartService.API.StartupRegistration;

public static class ApiVersioningRegistration
{
    public static IServiceCollection AddConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(x =>
        {
            x.DefaultApiVersion = new ApiVersion(1, 0);
            x.ReportApiVersions = true;
            x.AssumeDefaultVersionWhenUnspecified = true;
            x.UnsupportedApiVersionStatusCode = StatusCodes.Status400BadRequest;
        })
        .AddApiExplorer(x =>
        {
            x.GroupNameFormat = "'v'VVV";
            x.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
}