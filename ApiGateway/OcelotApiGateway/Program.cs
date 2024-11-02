using ApiGateway.ResilienceProvider;
using ApiGateway.StartupRegistration;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Observability.Registrations;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var routes = "Routes";
builder.Configuration.AddOcelotWithSwaggerSupport(options =>
{
    options.Folder = routes;
});

builder.Services
    .AddOtelConfiguration(builder.Environment, builder.Configuration)
    .AddOcelot(builder.Configuration)
    // .AddPolly<RetryProvider>(); QosProvider is not implemented yet
    // .AddCustomLoadBalancer<CustomLoadBalancer>();
    .AddPolly();

builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddJsonFile("ocelot.json", optional: true, reloadOnChange: true);
builder.Services.AddAuthenticationConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerForOcelotUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseOcelot().Wait();
await app.RunAsync();