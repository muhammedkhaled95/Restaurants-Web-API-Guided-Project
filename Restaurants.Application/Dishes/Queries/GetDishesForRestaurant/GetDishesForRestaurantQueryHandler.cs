using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Dishes.DTOs;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Queries.GetDishesForRestaurant;

public class GetDishesForRestaurantQueryHandler : IRequestHandler<GetDishesForRestaurantQuery, IEnumerable<DishDto>>
{
    private readonly ILogger<GetDishesForRestaurantQueryHandler> _logger;
    private readonly IRestaurantsRepository _restaurantsRepository;
    private readonly IMapper _mapper;
    public GetDishesForRestaurantQueryHandler(ILogger<GetDishesForRestaurantQueryHandler> logger,
                                              IRestaurantsRepository restaurantsRepository, IMapper mapper)
    {
        _logger = logger;
        _restaurantsRepository = restaurantsRepository;
        _mapper = mapper;
    }
    public async Task<IEnumerable<DishDto>> Handle(GetDishesForRestaurantQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting dishes for restaurant with id: {restaurantId}", request.RestaurantId);
        
        var restaurant = await _restaurantsRepository.GetByIdAsync(request.RestaurantId);

        if (restaurant == null) 
        {
            throw new ResourceNotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
        }

        var dishes = _mapper.Map<IEnumerable<DishDto>>(restaurant.dishes);
        return dishes;

    }
}
