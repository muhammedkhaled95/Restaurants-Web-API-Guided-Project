using Microsoft.OpenApi.Models;
using Restaurants.API.Middlewares;
using Serilog;

namespace Restaurants.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
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

        // 1. Configure Serilog for the host.
        //    - This sets up Serilog as the logging provider for the application.
        //    - The 'UseSerilog' method accepts a lambda where you can configure Serilog's settings.
        //    - The 'context' parameter can be used to read from configuration files (if desired).
        //    - The 'configuration' parameter is used to set up logging levels, sinks, enrichers, etc.
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
    }
}
