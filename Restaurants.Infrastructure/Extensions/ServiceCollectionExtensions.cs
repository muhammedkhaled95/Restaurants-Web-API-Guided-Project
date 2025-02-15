using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Domain.Repositories;
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
            services.AddDbContext<RestaurantsDbContext>(options =>
                // Configure EF Core to use SQL Server with the provided connection string.
                options.UseSqlServer(connectionString)
                       // EnableSensitiveDataLogging includes detailed parameter values and other sensitive data
                       // in exception messages and logs. This is very useful during development and debugging.
                       // However, it's important to disable this in production to avoid exposing sensitive data.
                       .EnableSensitiveDataLogging()
            );

            services.AddScoped<IRestaurantSeeder, RestaurantSeeder>();
            services.AddScoped<IRestaurantsRepository, RestaurantsRepository>();
        }
    }
}
