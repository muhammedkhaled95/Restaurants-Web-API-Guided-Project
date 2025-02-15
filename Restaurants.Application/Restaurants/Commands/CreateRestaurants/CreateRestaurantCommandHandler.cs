using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurants
{
    public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, int>
    {
        private readonly ILogger<CreateRestaurantCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IRestaurantsRepository _restaurantsRepository;

        public CreateRestaurantCommandHandler(ILogger<CreateRestaurantCommandHandler> logger, IMapper mapper, IRestaurantsRepository restaurantsRepository)
        {
            _logger = logger;
            _restaurantsRepository = restaurantsRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
        {
            // The @ symbol instructs Serilog (and some other logging libraries) to serialize the request object as structured data.
            // This way, the entire object can be easily queried or displayed in a structured log viewer.
            _logger.LogInformation("Creating a new restaurant {@restaurant}", request);

            var restaurant = _mapper.Map<Restaurant>(request);

            int id = await _restaurantsRepository.Create(restaurant);

            return id;
        }
    }
}
