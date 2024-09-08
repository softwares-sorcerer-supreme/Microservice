using AuthService.Application.StartupRegistration;
using AuthService.Infrastructure.StartupRegistration;
using AuthService.Persistence.StartupRegistration;
using Serilog;
using Shared.Middlewares;

namespace AuthService.API;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddRazorPages();

        builder.Services.AddAuthenticationConfiguration(builder.Configuration)
            .AddDatabaseConfiguration(builder.Configuration)
            .AddMediatorConfiguration()
            .AddValidatorConfiguration()
            .AddDIConfiguration()
            .AddOptionConfiguration(builder.Configuration);

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
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