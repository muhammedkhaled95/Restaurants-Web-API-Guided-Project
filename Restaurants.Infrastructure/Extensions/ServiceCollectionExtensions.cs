using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Authorization;
using Restaurants.Infrastructure.Authorization.Requirements;
using Restaurants.Infrastructure.Persistence;
using Restaurants.Infrastructure.Repositories;
using Restaurants.Infrastructure.Seeders;


namespace Restaurants.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("RestaurantsDBConnectionString");

            // Register the RestaurantsDbContext with the dependency injection container.
            // This tells EF Core how to connect to the database and provides additional options.
            services.AddDbContext<ApplicationDbContext>(options =>
                // Configure EF Core to use SQL Server with the provided connection string.
                options.UseSqlServer(connectionString)
                       // EnableSensitiveDataLogging includes detailed parameter values and other sensitive data
                       // in exception messages and logs. This is very useful during development and debugging.
                       // However, it's important to disable this in production to avoid exposing sensitive data.
                       .EnableSensitiveDataLogging()
            );

            // 📝 Identity with Entity Framework Configuration
            // ---------------------------------------------
            // This code sets up ASP.NET Identity with built-in API endpoints 
            // (e.g., /login, /register, /logout) and uses Entity Framework Core 
            // to persist Identity data in the ApplicationDbContext.
            //
            // 🔑 AddIdentityApiEndpoints<AppUser>:
            // - Registers default Identity endpoints without manual controllers.
            // - Handles authentication actions like login and registration automatically.
            //
            // 🗄️ AddEntityFrameworkStores<ApplicationDbContext>:
            // - Configures Identity to use EF Core for storing user, role, and token data.
            // - Manages tables like AspNetUsers, AspNetRoles, AspNetUserClaims, etc.
            //
            // ✅ Usage:
            // builder.Services.AddIdentityApiEndpoints<AppUser>()
            //         .AddEntityFrameworkStores<ApplicationDbContext>();
            //
            // ✅ Endpoint Mapping (in Program.cs):
            // app.MapIdentityApi<AppUser>();  // Maps the Identity endpoints
            //
            // 🚀 Result: 
            // - Ready-to-use endpoints for authentication and authorization.
            // - No need to manually write CRUD operations for Identity entities.
            services.AddIdentityApiEndpoints<User>()
                    /// <summary>
                    /// Registers role management services for ASP.NET Core Identity, enabling support for role-based authorization.
                    /// </summary>
                    /// <remarks>
                    /// Adds the necessary services to manage roles using <see cref="RoleManager{IdentityRole}"/> 
                    /// and enables the use of role-based attributes like <c>[Authorize(Roles = "admin")]</c>.
                    /// This method allows creating, updating, deleting, and assigning roles to users.
                    /// </remarks>
                    .AddRoles<IdentityRole>()
                    .AddClaimsPrincipalFactory<RestaurantsUserClaimsPrincipalFactory>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IRestaurantSeeder, RestaurantSeeder>();
            services.AddScoped<IRestaurantsRepository, RestaurantsRepository>();
            services.AddScoped<IDishesRepository, DishesRepository>();

            // Registers the authorization services into the dependency injection (DI) container.
            // This enables the application to use ASP.NET Core's built-in authorization system,
            // allowing you to define and enforce policies, roles, and claims-based access control.
            // This method prepares the app to handle [Authorize] attributes and custom authorization handlers.
            services.AddAuthorizationBuilder()
                    .AddPolicy(PolicyNames.HasNationality, builder => builder.RequireClaim(AppClaimTypes.Nationality, "German", "Egyptian"))
                    .AddPolicy(PolicyNames.AtLeast20YearsOfAge,
                    builder => builder.AddRequirements(new MinimumAgeRequirement(20)));

            services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();

        }
    }
}
