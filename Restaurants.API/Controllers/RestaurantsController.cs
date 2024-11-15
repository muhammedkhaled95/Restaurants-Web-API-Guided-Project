using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Restaurants;

namespace Restaurants.API.Controllers;

[ApiController]
[Route("api/restaurants")]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantsService restaurantsService;

    public RestaurantsController(IRestaurantsService restaurantsService)
    {
        this.restaurantsService = restaurantsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var restautants = await restaurantsService.GetAllRestaurants();

        return Ok(restautants);
    }
}
