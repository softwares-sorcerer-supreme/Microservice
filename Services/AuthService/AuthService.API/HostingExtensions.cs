using AuthService.API.StartupRegistration;
using AuthService.Application.StartupRegistration;
using AuthService.Infrastructure.StartupRegistration;
using AuthService.Persistence.StartupRegistration;
using Caching.StartupRegistration;
using Microsoft.OpenApi.Models;
using Observability.Registrations;
using Serilog;
using Shared.HttpContextCustom;
using Shared.Middlewares;

namespace AuthService.API;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddRazorPages();

        builder.Services.AddAuthenticationConfiguration(builder.Configuration)
            .AddDatabaseConfiguration(builder.Configuration)
            .AddMediatorConfiguration()
            .AddValidatorConfiguration()
            .AddDIConfiguration()
            .AddOptionConfiguration(builder.Configuration)
            .AddRedisConfiguration()
            .AddCustomHttpContextAccessor()
            .AddConfigureApiVersioning()
            .AddCustomSwaggerConfiguration();
        
        builder.Host.UseLogging(builder.Configuration);
        
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Authentication - V1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Authentication - V2");
            });            
            app.UseDeveloperExceptionPage();
        }
        
        app.UseMiddleware<ExceptionHandleMiddleware>();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        app.MapControllers();

        return app;
    }
}