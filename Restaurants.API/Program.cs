using Restaurants.Infrastructure.Extensions;
using Restaurants.Application.Extensions;
using Restaurants.Infrastructure.Seeders;
using Serilog;
using Serilog.Events;
using Restaurants.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

// Registering the error handling middleware as a dependency.
builder.Services.AddScoped<ErrorHandlingMiddleware>();

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

// Middleware in ASP.NET Core:
// Middleware components process requests and responses in a pipeline-like fashion.
// - They execute in the order they are added, affecting both incoming requests and outgoing responses.
// - Common use cases include logging, authentication, authorization, error handling, CORS, and request modifications.
// - Middleware can be built-in (e.g., UseRouting, UseAuthentication) or custom (via UseMiddleware<T>).
// - Use() passes control to the next middleware, while Run() ends the pipeline.
// - Order matters: middleware should be arranged correctly to ensure proper request handling.

// The following middleware components act as a bridge between incoming requests and endpoints,
// ensuring that every request passes through each middleware before reaching the controllers,
// and every response passes through them before reaching the client:
//
// - app.UseSerilogRequestLogging(): Logs every request using Serilog.
// - app.UseSwagger() and app.UseSwaggerUI(): Enable API documentation (active only in development).
// - app.UseHttpsRedirection(): Redirects HTTP requests to HTTPS.
// - app.UseAuthorization(): Enforces authorization policies on endpoints.
// - app.MapControllers(): Maps requests to controller actions.
//
// The order is crucial: each middleware component is executed in the order defined.
// No request or response bypasses these middleware steps unless explicitly handled within a middleware.

// Adding the error handling middleware as the first one in the http request pipeline for middlewares.
app.UseMiddleware<ErrorHandlingMiddleware>();

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
