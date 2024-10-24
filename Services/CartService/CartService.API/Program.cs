using CartService.API.StartupRegistration;
using CartService.Application.StartupRegistration;
using CartService.Domain.Abstraction;
using CartService.Infrastructure.StartupRegistration;
using CartService.Persistence;
using CartService.Persistence.MongoDB.StartupRegistration;
using CartService.Persistence.StartupRegistration;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services
    .AddDatabaseConfiguration(builder.Configuration)
    .AddValidators()
    .AddCustomMediator()
    .AddConfigureApiVersioning()
    .AddGrpcConfiguration(builder.Configuration)
    .AddAuthenticationConfiguration(builder.Configuration)
    .AddAuthorizationRegistration()
    .AddOptionConfiguration(builder.Configuration)
    .AddCustomSwaggerConfiguration()
    .AddMongoDbConfiguration()
    .AddHealthChecks();
    // .AddNpgSql(pgConnectionString)
    // .AddRedis(redisConnectionString);

    var loggerFactory = LoggerFactory.Create(
        loggingBuilder => loggingBuilder
            // add console as logging target
            .AddConsole()
            // add debug output as logging target
            .AddDebug()
            // set minimum level to log
            .SetMinimumLevel(LogLevel.Debug));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<ExceptionHandleMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
//HealthCheck Middleware

app.MapHealthChecks("/api/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();
