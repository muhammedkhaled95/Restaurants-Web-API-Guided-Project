using Restaurants.Infrastructure.Extensions;
using Restaurants.Application.Extensions;
using Restaurants.Infrastructure.Seeders;
using Serilog;
using Serilog.Events;
using Restaurants.API.Middlewares;
using Restaurants.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


// 📝 Swagger Configuration for JWT Authentication in API Documentation
// ---------------------------------------------------------------------
// ✅ Purpose:
// This block configures Swagger to recognize and use JWT Bearer tokens for API authentication.
// Without this setup, even if your API uses JWT, Swagger wouldn't know how to send tokens with requests.
//
// ✅ Why is this useful?
// - Enables the "Authorize" button in Swagger UI, allowing you to input a JWT token once
//   and automatically include it in all authorized requests.
// - Helps during development and testing without needing external tools like Postman.
//
// ---------------------------------------------------------------------
// 📦 1️⃣ AddSecurityDefinition("bearerAuth", ...)
// -----------------------------------------------
// - This registers a new security scheme named "bearerAuth" in Swagger.
// - Swagger uses this definition to understand how clients (like Swagger UI) should provide the JWT token.
//
// 🛡️ Parameters:
// - Type = SecuritySchemeType.Http: Specifies we're using an HTTP-based auth scheme.
// - Scheme = "Bearer": Indicates that the token should be passed in the Authorization header as a Bearer token.
//
// 🗣️ Example request header generated:
// Authorization: Bearer <your-jwt-token>
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    // -----------------------------------------------------------------
    // 📦 2️⃣ AddSecurityRequirement(...)
    // ----------------------------------
    // - This enforces the use of the "bearerAuth" scheme for API endpoints.
    // - Without this, Swagger would know about the token but wouldn't require it for any operations.
    //
    // 🔍 How it works:
    // - OpenApiSecurityRequirement is essentially a dictionary:
    //     Key   = OpenApiSecurityScheme (security definition reference)
    //     Value = List of scopes (empty for JWT, since scopes are optional and often unused)
    //
    // ✅ Benefits:
    // - Ensures endpoints protected by [Authorize] require a token in Swagger.
    // - Activates the "Authorize" button functionality in the Swagger UI.
    //
    // 🛡️ Key parts:
    // - Reference.Type = SecurityScheme: Tells Swagger we’re referencing a previously defined security scheme.
    // - Reference.Id = "bearerAuth": Matches the name defined above in AddSecurityDefinition.
    // - new string[] { }: Represents required scopes (empty for this case).
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, // 🔑 Refers to a security scheme definition
                    Id = "bearerAuth" // 🏷️ Must match the name used in AddSecurityDefinition
                }
            },
            new string[] { } // 🎯 No scopes required (common for JWT tokens without OAuth2 scopes)
        }
    });
});

// 📝 AddEndpointsApiExplorer (Commented Out)
// ------------------------------------------
// 🚫 This line is commented out because ASP.NET Core 7.0+ with Minimal APIs 
// automatically registers the API explorer, making endpoints visible in Swagger 
// without needing this service.
//
// ✅ Why it's safe to skip now:
// - Minimal APIs in .NET 7+ handle endpoint discovery by default.
// - Endpoints (including Identity and custom routes) already appear in Swagger.
//
// 🔑 When to re-enable:
// - If upgrading/downgrading to versions where endpoint discovery isn’t automatic.
// - Using controllers without AddControllers() and endpoints don’t show up.
// - Adding custom OpenAPI configurations that require manual registration.
//
// builder.Services.AddEndpointsApiExplorer(); // Uncomment if endpoints stop appearing in Swagger


// Registering the error handling middleware as a dependency.
builder.Services.AddScoped<ErrorHandlingMiddleware>();

// Registering the request time logging middleware as a dependency.
builder.Services.AddScoped<RequestTimeLoggingMiddleware>();

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

// Adding the request time logging middleware in case the request took more than 4 seconds to be done.
app.UseMiddleware<RequestTimeLoggingMiddleware>();

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


// 📝 Maps Identity API endpoints without the need for separate controller files.
// 📍 URL Prefix: Adds "api/identity" as a prefix to all Identity-related endpoints (e.g., /api/identity/login, /api/identity/register).
// 🔄 Automatically includes built-in endpoints for user authentication and account management provided by ASP.NET Identity.
// ✅ Benefits:
//   - Reduces boilerplate code by auto-generating routes for login, register, logout, etc.
//   - Ensures consistency with Identity conventions without writing explicit controllers.
//   - Makes Identity endpoints discoverable under a common URL prefix for easier management.
//
// 🛡️ Example generated endpoints:
//   - POST /api/identity/login
//   - POST /api/identity/register
//   - POST /api/identity/logout
//
// 🔔 TIP: Customize Identity options (like token lifespan or password requirements) in the Identity configuration section.
app.MapGroup("api/identity").MapIdentityApi<User>();


app.UseAuthorization();

app.MapControllers();

app.Run();
