using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;
using Restaurants.Domain.Constants;
namespace Restaurants.Application.Restaurants.Commands.DeleteRestaurants;

public class DeleteRestaurantCommandHandler : IRequestHandler<DeleteRestaurantCommand>
{
    private readonly ILogger<DeleteRestaurantCommandHandler> _logger;
    private readonly IRestaurantsRepository _restaurantsRepository;
    private readonly IRestaurantAuthorizationService _restaurantAuthorizationService;

    public DeleteRestaurantCommandHandler(ILogger<DeleteRestaurantCommandHandler> logger,
                                          IRestaurantsRepository restaurantsRepository,
                                          IRestaurantAuthorizationService restaurantAuthorizationService)
    {
        _logger = logger;
        _restaurantsRepository = restaurantsRepository;
        _restaurantAuthorizationService = restaurantAuthorizationService;
    }
    public async Task Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting Restaurant with id = {request.Id}", request.Id);
        var restaurant = await _restaurantsRepository.GetByIdAsync(request.Id);

        if (restaurant == null)
        {
            throw new ResourceNotFoundException(nameof(Restaurant), request.Id.ToString());
        }
        bool IsDeleteOperationAuthorized = _restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Delete);
        if (!IsDeleteOperationAuthorized)
        {
            throw new ForbidException();
        }
            
        await _restaurantsRepository.Delete(restaurant);
    }
}
