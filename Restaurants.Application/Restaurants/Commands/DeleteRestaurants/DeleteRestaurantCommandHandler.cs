using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Commands.CreateRestaurants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.DeleteRestaurants;

public class DeleteRestaurantCommandHandler : IRequestHandler<DeleteRestaurantCommand>
{
    private readonly ILogger<DeleteRestaurantCommandHandler> _logger;
    private readonly IRestaurantsRepository _restaurantsRepository;

    public DeleteRestaurantCommandHandler(ILogger<DeleteRestaurantCommandHandler> logger, IMapper mapper, IRestaurantsRepository restaurantsRepository)
    {
        _logger = logger;
        _restaurantsRepository = restaurantsRepository;
    }
    public async Task Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting Restaurant with id = {request.Id}", request.Id);
        var restaurant = await _restaurantsRepository.GetByIdAsync(request.Id);

        if (restaurant == null)
        {
            throw new ResourceNotFoundException(nameof(Restaurant), request.Id.ToString());
        }
        
        await _restaurantsRepository.Delete(restaurant);
    }
}
