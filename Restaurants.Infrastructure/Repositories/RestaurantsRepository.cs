using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;

namespace Restaurants.Infrastructure.Repositories;

internal class RestaurantsRepository(ApplicationDbContext dbContext) : IRestaurantsRepository
{
    public async Task<IEnumerable<Restaurant>> GetAllAsync()
    {
        var restaurants = await dbContext.Restaurants.ToListAsync();
        return restaurants;
    }

    public async Task<IEnumerable<Restaurant>> GetAllMatchingAsync(string? SearchPhrase)
    {
        var searchPhraseLower = SearchPhrase?.ToLower();

        var restaurants = await dbContext.Restaurants.Where(r => searchPhraseLower == null ||
                (!string.IsNullOrEmpty(r.Name) && r.Name.Contains(searchPhraseLower))
                || (!string.IsNullOrEmpty(r.Description) && r.Description.ToLower().Contains(searchPhraseLower))).ToListAsync();

        return restaurants;
    }

public async Task<Restaurant?> GetByIdAsync(int id)
{
    var restaurant = await dbContext.Restaurants.Include(restaurant => restaurant.dishes).FirstOrDefaultAsync(x => x.Id == id);
    return restaurant;
}

public async Task<int> Create(Restaurant restaurantEntity)
{
    /*
     * Why dbContext.Add is not asynchronous
     * =====================================
     * The dbContext.Add method in Entity Framework Core is not an asynchronous operation because it does not interact 
     * with the database when called. Instead, it only marks the entity (restaurantEntity in this case)
     * as being in the "Added" state in the DbContext's change tracker.
     * The actual interaction with the database happens when you call dbContext.SaveChangesAsync(). 
     * At that point, EF Core translates the changes into SQL commands and executes them against the database.
     */
    dbContext.Add(restaurantEntity);
    await dbContext.SaveChangesAsync();

    /*
     * How we got the Id from restaurantEntity 
     * =======================================
     * When you save an entity to the database using EF Core (via SaveChanges or SaveChangesAsync),
     * the database generates the Id if it's configured as an identity column (e.g., an auto-increment primary key).
     *                            ----------------------------------------------
     * Here’s what happens in detail: 
     * ==============================
     * Before SaveChangesAsync: The Id property of restaurantEntity is likely 0 (default for integers) or 
     * whatever value you set explicitly (if any).
     * During SaveChangesAsync: 
     * EF Core sends an INSERT statement to the database.
     * If the Id column in the database is an identity column, the database generates the new ID value.
     * After SaveChangesAsync:
     * EF Core retrieves the generated Id value from the database and updates the restaurantEntity object in memory with the new Id.
     */
    return restaurantEntity.Id;
}

public async Task Delete(Restaurant restaurantEntity)
{
    dbContext.Remove(restaurantEntity);
    await dbContext.SaveChangesAsync();
}

public async Task SaveChanges()
{
    await dbContext.SaveChangesAsync();
}
}