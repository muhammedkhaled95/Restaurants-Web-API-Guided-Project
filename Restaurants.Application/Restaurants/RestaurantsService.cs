using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants;

internal class RestaurantsService : IRestaurantsService
{
    private readonly IRestaurantsRepository _restaurantsRepository;
    private readonly ILogger<RestaurantsService> _logger;
    
    public RestaurantsService(IRestaurantsRepository restaurantsRepository, ILogger<RestaurantsService> logger)
    {
        _logger = logger;
        _restaurantsRepository = restaurantsRepository;
    }
    public async Task<IEnumerable<Restaurant>> GetAllRestaurants()
    {
        _logger.LogInformation("Getting All Restaurants");
        var restaurants = await _restaurantsRepository.GetAllAsync();
        return restaurants;
    }

    public async Task<Restaurant> GetRestaurantById(int id)
    {
        _logger.LogInformation($"Getting Restaurant with id = {id}");
        
        var restaurant = await _restaurantsRepository.GetByIdAsync(id);
        return restaurant;
    }
}
