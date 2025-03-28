using CartService.API.Registrations;
using CartService.Application.StartupRegistration;
using CartService.Domain.Abstraction;
using CartService.Infrastructure.StartupRegistration;
using CartService.Persistence;
using CartService.Persistence.MongoDB.StartupRegistration;
using CartService.Persistence.StartupRegistration;
using EventMessage.Registrations;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Observability.Middlewares;
using Observability.Registrations;
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
    .AddServicesConfiguration(builder.Configuration)
    .AddAuthenticationConfiguration(builder.Configuration)
    .AddAuthorizationRegistration()
    .AddOptionConfiguration(builder.Configuration)
    .AddCustomSwaggerConfiguration()
    .AddMongoDbConfiguration()
    .AddMassTransitConfiguration()
    .AddOtelConfiguration(builder.Environment, builder.Configuration)
    .AddHealthChecks();
// .AddNpgSql(pgConnectionString)
// .AddRedis(redisConnectionString);
builder.Host.UseLogging(builder.Configuration);
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

// app.UseMassTransitHealthCheck();
app.UseMiddleware<LogContextMiddleware>();
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