using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Commands.CreateRestaurants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurants;

public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand>
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
    public async Task Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        // The @ symbol instructs Serilog (and some other logging libraries) to serialize the request object as structured data.
        // This way, the entire object can be easily queried or displayed in a structured log viewer.
        _logger.LogInformation("Updating Restaurant with id = {request.Id} with {@restaurant}", request.Id, request);
        var restaurant = await _restaurantsRepository.GetByIdAsync(request.Id);

        if (restaurant == null)
        {
            throw new ResourceNotFoundException(nameof(Restaurant), request.Id.ToString());
        }

        // Using the auto mapper instead of manually mapping the request to the restaurant as in the commented out part.
        _mapper.Map(request, restaurant);
        /*
        restaurant.Name = request.Name;
        restaurant.Description = request.Description;
        restaurant.HasDelivery = request.HasDelivery;
        */
        await _restaurantsRepository.SaveChanges();
    }
}
