using AutoMapper;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.DTOs;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants;

internal class RestaurantsService : IRestaurantsService
{
    private readonly IRestaurantsRepository _restaurantsRepository;
    private readonly ILogger<RestaurantsService> _logger;
    private readonly IMapper _mapper;
    
    public RestaurantsService(IRestaurantsRepository restaurantsRepository, ILogger<RestaurantsService> logger,
                              IMapper mapper)
    {
        _logger = logger;
        _restaurantsRepository = restaurantsRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RestaurantDto>> GetAllRestaurants()
    {
        _logger.LogInformation("Getting All Restaurants");
        var restaurants = await _restaurantsRepository.GetAllAsync();

        // Manual Mapping the returned restaurants entities to a Dto to be returned to the controller.
        //var restaurantsDto = restaurants.Select(restaurant => RestaurantDto.MapEntityToDto(restaurant));

        // Auto mapping the returned restaurants entities to a Dto to be returned to the controller.
        var restaurantsDto = _mapper.Map<IEnumerable<RestaurantDto>>(restaurants);

        return restaurantsDto!;
    }

    public async Task<RestaurantDto?> GetRestaurantById(int id)
    {
        _logger.LogInformation($"Getting Restaurant with id = {id}");
        
        var restaurant = await _restaurantsRepository.GetByIdAsync(id);

        // Manual Mapping the returned restaurant entity to a Dto to be returned to the controller.
        //var restaurantDto = RestaurantDto.MapEntityToDto(restaurant);

        // Auto mapping the returned restaurant entity to a Dto to be returned to the controller.
        var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);

        return restaurantDto;
    }

    public async Task<int> CreateRestaurant(CreateRestaurantDto createRestaurantDto)
    {
        _logger.LogInformation("Creating a new restaurant (resource)");
         
        var restaurant = _mapper.Map<Restaurant>(createRestaurantDto);
        
        int id = await _restaurantsRepository.Create(restaurant);

        return id;
    }

}
