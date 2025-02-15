using Restaurants.Infrastructure.Extensions;
using Restaurants.Application.Extensions;
using Restaurants.Infrastructure.Seeders;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

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
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

var scope = app.Services.CreateScope();

var seeder = scope.ServiceProvider.GetRequiredService<IRestaurantSeeder>();

await seeder.Seed();

// 4. Use Serilog's request logging middleware.
//    - This middleware automatically logs HTTP request information such as the HTTP method,
//      request path, status code, and the time taken to process the request.
app.UseSerilogRequestLogging();

// Using Swagger docs onl in case of development environment.
if (app.Environment.IsDevelopment())
{
    // Using Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
