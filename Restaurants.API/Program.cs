using Restaurants.Infrastructure.Extensions;
using Restaurants.Application.Extensions;
using Restaurants.Infrastructure.Seeders;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Adding all the services in the DI container from the infrastructure Layer.
builder.Services.AddInfrastructure(builder.Configuration);

// Adding all the services in the DI container from the application layer.
builder.Services.AddApplication();

// 1. Configure Serilog for the host.
//    - This sets up Serilog as the logging provider for the application.
//    - The 'UseSerilog' method accepts a lambda where you can configure Serilog's settings.
//    - The 'context' parameter can be used to read from configuration files (if desired).
//    - The 'configuration' parameter is used to set up logging levels, sinks, enrichers, etc.
builder.Host.UseSerilog((context, configuration) =>
{
    // 2. Override default logging levels for specific namespaces.
    //    - "Microsoft" logs will only be logged if they are Warning level or above.
    //    - "Microsoft.EntityFrameworkCore" logs will be at the Information level or above.
    //    - This helps reduce noise in logs by ignoring lower-level logs from the framework.
    configuration
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)

        // 3. Write logs to the Console sink.
        //    - You can add additional sinks here, e.g. .WriteTo.File(...) or .WriteTo.Seq(...).
        .WriteTo.Console(outputTemplate: "[{Timestamp: dd-MM HH:mm:ss} {Level:u3}] |{SourceContext}|{NewLine}{Message:lj}{NewLine}{Exception}");
});

var app = builder.Build();

var scope = app.Services.CreateScope();

var seeder = scope.ServiceProvider.GetRequiredService<IRestaurantSeeder>();

await seeder.Seed();

// 4. Use Serilog's request logging middleware.
//    - This middleware automatically logs HTTP request information such as the HTTP method,
//      request path, status code, and the time taken to process the request.
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
