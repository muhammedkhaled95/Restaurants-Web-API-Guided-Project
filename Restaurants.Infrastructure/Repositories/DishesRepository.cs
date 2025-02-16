using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;

namespace Restaurants.Infrastructure.Repositories;

class DishesRepository(ApplicationDbContext dbContext) : IDishesRepository
{
    public async Task<int> Create(Dish dishEntity)
    {
        dbContext.Dishes.Add(dishEntity);
        await dbContext.SaveChangesAsync();
        return dishEntity.Id;
    }
}
