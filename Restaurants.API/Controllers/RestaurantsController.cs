using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Restaurants;
using Restaurants.Application.Restaurants.DTOs;
using System.Formats.Asn1;

namespace Restaurants.API.Controllers;

[ApiController]
[Route("api/restaurants")]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantsService _restaurantsService;

    public RestaurantsController(IRestaurantsService restaurantsService)
    {
        _restaurantsService = restaurantsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var restautants = await _restaurantsService.GetAllRestaurants();

        return Ok(restautants);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetRestaurantById([FromRoute] int id)
    {
        var restaurant = await _restaurantsService.GetRestaurantById(id);

        if (restaurant == null)
        {
            return NotFound();
        } else
        {
            return Ok(restaurant);
        }
        
    }

    [HttpPost]
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantDto createRestaurantDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int id = await _restaurantsService.CreateRestaurant(createRestaurantDto);

        // Returns a 201 Created response with the Location header pointing to the GetRestaurantById action.
        // - `CreatedAtAction` specifies the action used to generate the URL for the newly created resource.
        // - `nameof(GetRestaurantById)` ensures the action name is dynamically referenced, reducing errors during refactoring.
        // - `new { id }` creates a route value for the URL by passing the ID of the new resource.
        // - The response body is set to `null` (no content); modify this if you want to include additional data in the response.
        return CreatedAtAction(nameof(GetRestaurantById), new { id }, null);

    }
}
