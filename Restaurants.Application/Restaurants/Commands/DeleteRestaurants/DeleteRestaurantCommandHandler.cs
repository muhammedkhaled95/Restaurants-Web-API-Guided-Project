using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Commands.CreateRestaurants;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.DeleteRestaurants;

public class DeleteRestaurantCommandHandler : IRequestHandler<DeleteRestaurantCommand, bool>
{
    private readonly ILogger<DeleteRestaurantCommandHandler> _logger;
    private readonly IRestaurantsRepository _restaurantsRepository;

    public DeleteRestaurantCommandHandler(ILogger<DeleteRestaurantCommandHandler> logger, IMapper mapper, IRestaurantsRepository restaurantsRepository)
    {
        _logger = logger;
        _restaurantsRepository = restaurantsRepository;
    }
    public async Task<bool> Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Deleting Restaurant with id = {request.Id}");
        var restaurant = await _restaurantsRepository.GetByIdAsync(request.Id);

        if (restaurant == null)
        {
            return false;
        }
        
        await _restaurantsRepository.Delete(restaurant);
        return true;
    }
}
