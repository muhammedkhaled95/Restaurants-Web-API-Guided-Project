using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Commands.CreateRestaurants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurants;

public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand, bool>
{
    private readonly ILogger<UpdateRestaurantCommandHandler> _logger;
    private readonly IRestaurantsRepository _restaurantsRepository;
    private readonly IMapper _mapper;

    public UpdateRestaurantCommandHandler(ILogger<UpdateRestaurantCommandHandler> logger, IMapper mapper, IRestaurantsRepository restaurantsRepository)
    {
        _mapper = mapper;
        _logger = logger;
        _restaurantsRepository = restaurantsRepository;
    }
    public async Task<bool> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating Restaurant with id = {request.Id}");
        var restaurant = await _restaurantsRepository.GetByIdAsync(request.Id);

        if (restaurant == null)
        {
            return false;
        }

        // Using the auto mapper instead of manually mapping the request to the restaurant as in the commented out part.
        _mapper.Map(request, restaurant);
        /*
        restaurant.Name = request.Name;
        restaurant.Description = request.Description;
        restaurant.HasDelivery = request.HasDelivery;
        */
        await _restaurantsRepository.SaveChanges();
        return true;
    }
}
