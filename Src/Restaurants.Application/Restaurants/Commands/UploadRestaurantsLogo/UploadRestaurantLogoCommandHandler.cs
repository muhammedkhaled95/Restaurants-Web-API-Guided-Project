using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.UploadRestaurantsLogo;

public class UploadRestaurantLogoCommandHandler : IRequestHandler<UploadRestaurantLogoCommand>
{
    private readonly ILogger<UploadRestaurantLogoCommandHandler> _logger;
    private readonly IRestaurantsRepository _restaurantsRepository;
    private readonly IRestaurantAuthorizationService _restaurantAuthorizationService;
    private readonly IBlobStorageService _blobStorageService;
    public UploadRestaurantLogoCommandHandler(ILogger<UploadRestaurantLogoCommandHandler> logger,
                                              IRestaurantsRepository restaurantsRepository,
                                              IRestaurantAuthorizationService restaurantAuthorizationService,
                                              IBlobStorageService blobStorageService)
    {
        _logger = logger;
        _restaurantsRepository = restaurantsRepository;
        _restaurantAuthorizationService = restaurantAuthorizationService;
        _blobStorageService = blobStorageService;
    }
    public async Task Handle(UploadRestaurantLogoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating Restaurant logo for restaurant with id = {request.Id} with {@restaurant}", request.RestaurantId, request);
        var restaurant = await _restaurantsRepository.GetByIdAsync(request.RestaurantId);

        if (restaurant == null)
        {
            throw new ResourceNotFoundException(nameof(Restaurant), request.RestaurantId.ToString());
        }

        bool IsUpdateOperationAuthorized = _restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Update);
        if (!IsUpdateOperationAuthorized)
        {
            throw new ForbidException();
        }

        var uploadedRestaurantLogoUrl = await _blobStorageService.UploadToBlobAsync(request.File, request.FileName);

        restaurant.LogoUrl  = uploadedRestaurantLogoUrl;

        await _restaurantsRepository.SaveChanges();
    }
}
