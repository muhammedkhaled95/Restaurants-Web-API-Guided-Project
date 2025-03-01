using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;
namespace Restaurants.Application.Dishes.Commands.CreateDishes;

public class CreateDishCommandHandler : IRequestHandler<CreateDishCommand, int>
{
    private readonly ILogger<CreateDishCommandHandler> _logger;
    private readonly IRestaurantsRepository _restaurantsRepository;
    private readonly IDishesRepository _dishesRepository;
    private readonly IMapper _mapper;
    private readonly IRestaurantAuthorizationService _restaurantAuthorizationService;
    public CreateDishCommandHandler(ILogger<CreateDishCommandHandler> logger, IRestaurantsRepository restaurantsRepository,
                                    IDishesRepository dishesRepository, IMapper mapper,
                                    IRestaurantAuthorizationService restaurantAuthorizationService)
    {
        _logger = logger;
        _restaurantsRepository = restaurantsRepository;
        _dishesRepository = dishesRepository;
        _mapper = mapper;
        _restaurantAuthorizationService = restaurantAuthorizationService;
    }


    public async Task<int> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new Dish: {@CreateDishRequest}", request);

        var restaurant = await _restaurantsRepository.GetByIdAsync(request.RestaurantId);
        if (restaurant == null) 
        {
            throw new ResourceNotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
        }

        // Check if updating a restaurant by creating a new dish is authorized or not.
        bool IsUpdateOperationAuthorized = _restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Update);
        if (!IsUpdateOperationAuthorized)
        {
            throw new ForbidException();
        }

        var dish = _mapper.Map<Dish>(request);

        return await _dishesRepository.Create(dish);

    }
}
