using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Dishes.DTOs;
using Restaurants.Application.Dishes.Queries.GetDishesForRestaurant;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Queries.GetDishByIdForRestaurant;

public class GetDishByIdForRestaurantQueryHandler : IRequestHandler<GetDishByIdForRestaurantQuery, DishDto>
{
    private readonly ILogger<GetDishesForRestaurantQueryHandler> _logger;
    private readonly IRestaurantsRepository _restaurantsRepository;
    private readonly IMapper _mapper;
    public GetDishByIdForRestaurantQueryHandler(ILogger<GetDishesForRestaurantQueryHandler> logger,
                                              IRestaurantsRepository restaurantsRepository, IMapper mapper)
    {
        _logger = logger;
        _restaurantsRepository = restaurantsRepository;
        _mapper = mapper;
    }
    public async Task<DishDto> Handle(GetDishByIdForRestaurantQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting dish with id: {dishId} for restaurant with id: {restaurantId}",request.DishId, request.RestaurantId);

        var restaurant = await _restaurantsRepository.GetByIdAsync(request.RestaurantId);

        if (restaurant == null)
        {
            throw new ResourceNotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
        }

        var dish = restaurant.dishes.FirstOrDefault(d => d.Id == request.DishId);

        if (dish == null) 
        {
            throw new ResourceNotFoundException(nameof(Dish), request.DishId.ToString());
        }

        var result = _mapper.Map<DishDto>(dish);
        return result;
    }
}
