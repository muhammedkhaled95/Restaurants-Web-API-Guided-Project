using Microsoft.Extensions.Logging;
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Interfaces;

namespace Restaurants.Infrastructure.Authorization.Services;

public class RestaurantAuthorizationService : IRestaurantAuthorizationService
{
    private readonly ILogger<RestaurantAuthorizationService> _logger;
    private readonly IUserContext _userContext;
    public RestaurantAuthorizationService(ILogger<RestaurantAuthorizationService> logger,
                                          IUserContext userContext)
    {
        _logger = logger;
        _userContext = userContext;
    }

    public bool Authorize(Restaurant restaurant, ResourceOperation operation)
    {
        var user = _userContext.GetCurrentUser();
        _logger.LogInformation("Authorizing User {UserEmail} to {operation} Restaurant {RestaurantName}",
                               user?.Email, operation, restaurant.Name);

        if (operation == ResourceOperation.Read || operation == ResourceOperation.Create)
        {
            _logger.LogInformation("Creating or Reading Resource, Successful Authorization");
            return true;
        }
        if (operation == ResourceOperation.Delete && user!.IsInRole(UserRoles.Admin))
        {
            _logger.LogInformation("Admin user deleting a resource, Successful Authorization");
            return true;
        }
        if ((operation == ResourceOperation.Delete || operation == ResourceOperation.Update)
            && user!.Id == restaurant.OwnerId)
        {
            _logger.LogInformation("Resource owner to delete or update a resouce, Successful Authorization");
            return true;
        }

        return false;
    }
}