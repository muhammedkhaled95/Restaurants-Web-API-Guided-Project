using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.DTOs;
using Restaurants.Application.Restaurants.Queries.GetAllRestaurants;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Queries.GetRestaurantById;

public class GetRestaurantByIdQueryHandler : IRequestHandler<GetRestaurantByIdQuery, RestaurantDto?>
{
    private readonly ILogger<GetRestaurantByIdQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRestaurantsRepository _restaurantsRepository;

    public GetRestaurantByIdQueryHandler(IRestaurantsRepository restaurantsRepository, ILogger<GetRestaurantByIdQueryHandler> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
        _restaurantsRepository = restaurantsRepository;
        
    }

    public async Task<RestaurantDto> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting Restaurant with id = {request.Id}");

        var restaurant = await _restaurantsRepository.GetByIdAsync(request.Id);

        // Manual Mapping the returned restaurant entity to a Dto to be returned to the controller.
        //var restaurantDto = RestaurantDto.MapEntityToDto(restaurant);

        // Auto mapping the returned restaurant entity to a Dto to be returned to the controller.
        var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);

        return restaurantDto;
    }
}
