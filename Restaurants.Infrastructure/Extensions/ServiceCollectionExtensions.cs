using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Infrastructure.Persistence;
using Restaurants.Infrastructure.Seeders;


namespace Restaurants.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("RestaurantsDBConnectionString");
            services.AddDbContext<RestaurantsDbContext>(Options => Options.UseSqlServer(connectionString));

            services.AddScoped<IRestaurantSeeder, RestaurantSeeder>();
        }
    }
}
