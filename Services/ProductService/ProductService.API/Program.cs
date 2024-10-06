using ProductService.API.StartupRegistration;
using ProductService.Application.StartupRegistration;
using ProductService.Infrastructure.StartupRegistration;
using ProductService.Persistence.StartupRegistration;
using Shared.Middlewares;

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
    .AddCustomMediator()
    .AddConfigureApiVersioning()
    .AddGrpcConfiguration()
    .AddAuthenticationConfiguration(builder.Configuration)
    .AddOptionConfiguration(builder.Configuration)
    .AddHttpClientCustom(builder.Configuration)
    .AddDIConfiguration();
    // .AddConfigureLogging(builder)

    var loggerFactory = LoggerFactory.Create(
        builder => builder
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
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcConfiguration();

app.Run();
