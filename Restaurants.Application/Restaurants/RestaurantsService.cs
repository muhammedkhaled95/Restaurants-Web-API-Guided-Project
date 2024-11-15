using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.DTOs;
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
    public async Task<IEnumerable<RestaurantDto>> GetAllRestaurants()
    {
        _logger.LogInformation("Getting All Restaurants");
        var restaurants = await _restaurantsRepository.GetAllAsync();

        // Mapping the returned restaurants entities to a Dto to be returned to the controller.
        var restaurantsDto = restaurants.Select(restaurant => RestaurantDto.MapEntityToDto(restaurant));
        return restaurantsDto!;
    }

    public async Task<RestaurantDto?> GetRestaurantById(int id)
    {
        _logger.LogInformation($"Getting Restaurant with id = {id}");
        
        var restaurant = await _restaurantsRepository.GetByIdAsync(id);

        // Mapping the returned restaurant entity to a Dto to be returned to the controller.
        var restaurantDto = RestaurantDto.MapEntityToDto(restaurant);
        return restaurantDto;
    }
}
