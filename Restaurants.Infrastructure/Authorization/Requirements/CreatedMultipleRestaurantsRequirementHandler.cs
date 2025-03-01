using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Users;
using Restaurants.Domain.Repositories;

namespace Restaurants.Infrastructure.Authorization.Requirements;

internal class CreatedMultipleRestaurantsRequirementHandler : AuthorizationHandler<CreatedMultipleRestaurantsRequirement>
{
    private readonly IRestaurantsRepository _restaurantsRepository;
    private readonly IUserContext _userContext;
    private readonly ILogger<CreatedMultipleRestaurantsRequirementHandler> _logger;
    public CreatedMultipleRestaurantsRequirementHandler(IRestaurantsRepository restaurantsRepository,
                                                        IUserContext userContext,
                                                        ILogger<CreatedMultipleRestaurantsRequirementHandler> logger)
    {
        _restaurantsRepository = restaurantsRepository;
        _userContext = userContext;
        _logger = logger;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                         CreatedMultipleRestaurantsRequirement requirement)
    {
        var currentUser = _userContext.GetCurrentUser();
        var restaurants = await _restaurantsRepository.GetAllAsync();

        var userRestaurantsCreatedCount = restaurants.Count(r => r.OwnerId == currentUser!.Id);

        if (userRestaurantsCreatedCount >= requirement.MinimumRestaurantsCreated)
        {
            context.Succeed(requirement);
        }
        else 
        {
            context.Fail();
        }
    }
}
