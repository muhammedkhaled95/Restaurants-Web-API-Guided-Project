using Microsoft.AspNetCore.Authorization;

namespace Restaurants.Infrastructure.Authorization.Requirements;

public class CreatedMultipleRestaurantsRequirement : IAuthorizationRequirement
{
    public CreatedMultipleRestaurantsRequirement(int minimumRestaurantsCreated)
    {
        MinimumRestaurantsCreated = minimumRestaurantsCreated;
    }
    public int MinimumRestaurantsCreated { get; set; }
}
