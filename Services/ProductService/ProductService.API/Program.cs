using ProductService.API.StartupRegistration;
using ProductService.Application.StartupRegistration;
using ProductService.Infrastructure.Middleware;
using ProductService.Persistence.StartupRegistration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddDatabaseConfiguration(builder.Configuration)
    .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
    .AddValidators()
    .AddCustomMediatR()
    .AddConfigureApiVersioning()
    .AddGrpcConfiguration();
    // .AddConfigureLogging(builder)

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandleMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcConfiguration();

app.Run();
