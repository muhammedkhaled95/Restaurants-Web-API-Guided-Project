using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Infrastructure.Persistence;


namespace Restaurants.Infrastructure.Seeders
{
    internal class RestaurantSeeder(ApplicationDbContext dbContext) : IRestaurantSeeder
    {
        public async Task Seed()
        {
            if (await dbContext.Database.CanConnectAsync())
            {
                if (dbContext.Database.GetPendingMigrations().Any()) 
                {
                    await dbContext.Database.MigrateAsync();
                }
                if (!dbContext.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();
                    await dbContext.Restaurants.AddRangeAsync(restaurants);
                    await dbContext.SaveChangesAsync();
                }

                if (!dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    await dbContext.Roles.AddRangeAsync(roles);
                    await dbContext.SaveChangesAsync();
                }

            }
        }

        private IEnumerable<IdentityRole> GetRoles() 
        {
            List<IdentityRole> roles = 
             [
                new (UserRoles.User) 
                {
                    NormalizedName = UserRoles.User.ToUpper()
                },
                new (UserRoles.Owner)
                {
                    NormalizedName = UserRoles.Owner.ToUpper()
                },
                new (UserRoles.Admin)
                {
                    NormalizedName = UserRoles.Admin.ToUpper()
                }
             ];

            return roles;
        }
        private IEnumerable<Restaurant> GetRestaurants()
        {
            User seedOwner = new User()
            {
                Email =  "seed-user@test.com"
            };    

            List<Restaurant> restaurants = [
                new()
            {
                Owner = seedOwner,
                Name = "KFC",
                Category = "Fast Food",
                Description =
                    "KFC (short for Kentucky Fried Chicken) is an American fast food restaurant chain headquartered in Louisville, Kentucky, that specializes in fried chicken.",
                ContactEmail = "contact@kfc.com",
                ContactNumber = "543211",
                HasDelivery = true,
                dishes =
                [
                    new ()
                    {
                        Name = "Nashville Hot Chicken",
                        Description = "Nashville Hot Chicken (10 pcs.)",
                        Price = 10.30M,
                    },

                    new ()
                    {
                        Name = "Chicken Nuggets",
                        Description = "Chicken Nuggets (5 pcs.)",
                        Price = 5.30M,
                    },
                ],
                Address = new ()
                {
                    City = "London",
                    Street = "Cork St 5",
                    PostalCode = "WC2N 5DU"
                },

            },
                new ()
            {
                Owner = seedOwner,
                Name = "McDonald",
                Category = "Fast Food",
                Description =
                    "McDonald's Corporation (McDonald's), incorporated on December 21, 1964, operates and franchises McDonald's restaurants.",
                ContactEmail = "contact@mcdonald.com",
                ContactNumber = "123456",
                HasDelivery = true,
                Address = new Address()
                {
                    City = "London",
                    Street = "Boots 193",
                    PostalCode = "W1F 8SR"
                }
            }
            ];

            return restaurants;
        }


    }

}