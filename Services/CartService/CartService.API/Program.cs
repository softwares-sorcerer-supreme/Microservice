using CartService.API.StartupRegistration;
using CartService.Application.StartupRegistration;
using CartService.Infrastructure.StartupRegistration;
using CartService.Persistence.StartupRegistration;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Shared.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddDatabaseConfiguration(builder.Configuration)
    .AddValidators()
    .AddCustomMediator()
    .AddConfigureApiVersioning()
    .AddGrpcConfiguration(builder.Configuration)
    .AddAuthenticationConfiguration(builder.Configuration)
    .AddOptionConfiguration(builder.Configuration)
    .AddHealthChecks();
    // .AddNpgSql(pgConnectionString)
    // .AddRedis(redisConnectionString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<ExceptionHandleMiddleware>();
app.UseAuthorization();
//HealthCheck Middleware

app.MapHealthChecks("/api/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();
