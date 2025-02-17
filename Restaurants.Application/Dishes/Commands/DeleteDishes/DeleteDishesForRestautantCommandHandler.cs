using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Commands.DeleteDishes;

public class DeleteDishesForRestautantCommandHandler : IRequestHandler<DeleteDishesForRestautantCommand>
{
    private readonly ILogger<DeleteDishesForRestautantCommandHandler> _logger;
    private readonly IRestaurantsRepository _restaurantsRepository;
    private readonly IDishesRepository _dishesRepository;
    public DeleteDishesForRestautantCommandHandler(ILogger<DeleteDishesForRestautantCommandHandler> logger,
                                                   IRestaurantsRepository restaurantsRepository,
                                                   IDishesRepository dishesRepository)
    {
        _logger = logger;
        _restaurantsRepository = restaurantsRepository;
        _dishesRepository = dishesRepository;
    }
    public async Task Handle(DeleteDishesForRestautantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting all the dishes from the restaurant with id {restaurantId}", request.RestaurantId);
        
        var restaurant = await _restaurantsRepository.GetByIdAsync(request.RestaurantId);

        if (restaurant == null) 
        {
            throw new ResourceNotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
        }

        await _dishesRepository.Delete(restaurant.dishes);
    }
}
