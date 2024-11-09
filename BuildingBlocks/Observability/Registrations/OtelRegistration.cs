using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Observability.Registrations;

public static class OtelRegistration
{
    public static IServiceCollection AddOtelConfiguration(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
    {
        var otelExporterOltEndpoint = configuration.GetSection("OTEL_EXPORTER_OTLP_ENDPOINT").Value;

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName: environment.ApplicationName))
            .AddMetricsTelemetry(otelExporterOltEndpoint)
            .AddTracingTelemetry(otelExporterOltEndpoint);

        return services;
    }

    // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
    private static IOpenTelemetryBuilder AddTracingTelemetry(this IOpenTelemetryBuilder otel, string? tracingOtlpEndpoint)
    {
        return otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation(options =>
            {
                // to trace only api requests
                // This is a predicate to filter requests by the path that determines whether a given HTTP context should be traced. Here, it’s set to only trace requests that contain “api” in the path.
                options.Filter = (context) => !string.IsNullOrEmpty(context.Request.Path.Value) && context.Request.Path.Value.Contains("api", StringComparison.InvariantCulture);

                // example: only collect telemetry about HTTP GET requests
                // return httpContext.Request.Method.Equals("GET");

                // enrich activity with http request and response
                // This enriches the activity with additional information from the HTTP request. Here, it’s adding a tag with the request protocol.
                options.EnrichWithHttpRequest = (activity, httpRequest) => { activity.SetTag("requestProtocol", httpRequest.Protocol); };
                // This enriches the activity with additional information from the HTTP response. Here, it’s adding a tag with the response length.
                options.EnrichWithHttpResponse = (activity, httpResponse) => { activity.SetTag("responseLength", httpResponse.ContentLength); };

                // automatically sets Activity Status to Error if an unhandled exception is thrown
                // This is a setting that determines whether unhandled exceptions should be automatically recorded.
                options.RecordException = true;

                //This enriches the activity with additional information from any unhandled exceptions. Here, it’s adding tags with the exception type and stack trace.
                options.EnrichWithException = (activity, exception) =>
                {
                    activity.SetTag("exceptionType", exception.GetType().ToString());
                    activity.SetTag("stackTrace", exception.StackTrace);
                };
            });

            tracing.AddHttpClientInstrumentation()
                .AddRedisInstrumentation()
                .AddNpgsql();

            tracing.AddEntityFrameworkCoreInstrumentation(options =>
            {
                options.SetDbStatementForText = true;
                options.SetDbStatementForStoredProcedure = true;
                options.EnrichWithIDbCommand = (activity, command) =>
                {
                    // var stateDisplayName = $"{command.CommandType} main";
                    // activity.DisplayName = stateDisplayName;
                    // activity.SetTag("db.name", stateDisplayName);
                };
            });

            if (!string.IsNullOrEmpty(tracingOtlpEndpoint))
            {
                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
                });
            }
            else
            {
                tracing.AddConsoleExporter();
            }
        });
    }

    // Add Metrics for ASP.NET Core and export to Prometheus
    private static IOpenTelemetryBuilder AddMetricsTelemetry(this IOpenTelemetryBuilder otel, string? metricsOtlpEndpoint)
    {
        return otel.WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddOtlpExporter(otlpOptions =>
            {
                if (!string.IsNullOrEmpty(metricsOtlpEndpoint))
                {
                    otlpOptions.Endpoint = new Uri(metricsOtlpEndpoint);
                }
            }));
    }

    /// <summary>
    /// Use when use the Prometheus exporter
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseOpenTelemetryPrometheusScrapingEndpoint(this IApplicationBuilder app)
    {
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
        return app;
    }
}