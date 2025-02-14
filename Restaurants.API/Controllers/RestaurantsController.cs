using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Restaurants;
using Restaurants.Application.Restaurants.Commands.CreateRestaurants;
using Restaurants.Application.Restaurants.Commands.DeleteRestaurants;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurants;
using Restaurants.Application.Restaurants.Queries.GetAllRestaurants;
using Restaurants.Application.Restaurants.Queries.GetRestaurantById;
using System.Formats.Asn1;

namespace Restaurants.API.Controllers;

[ApiController]
[Route("api/restaurants")]
public class RestaurantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RestaurantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var restautants = await _mediator.Send(new GetAllRestaurantsQuery());

        return Ok(restautants);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetRestaurantById([FromRoute] int id)
    {
        var restaurant = await _mediator.Send(new GetRestaurantByIdQuery(id));

        if (restaurant == null)
        {
            return NotFound();
        } else
        {
            return Ok(restaurant);
        }
        
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteRestaurantById([FromRoute] int id)
    {
        bool isRestaurantDeleted = await _mediator.Send(new DeleteRestaurantCommand(id));

        if (isRestaurantDeleted)
        {
            return NoContent();
        }
        else
        {
            return NotFound();
        }

    }

    [HttpPost]
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantCommand createRestaurantCommand)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int id = await _mediator.Send(createRestaurantCommand);

        // Returns a 201 Created response with the Location header pointing to the GetRestaurantById action.
        // - `CreatedAtAction` specifies the action used to generate the URL for the newly created resource.
        // - `nameof(GetRestaurantById)` ensures the action name is dynamically referenced, reducing errors during refactoring.
        // - `new { id }` creates a route value for the URL by passing the ID of the new resource.
        // - The response body is set to `null` (no content); modify this if you want to include additional data in the response.
        return CreatedAtAction(nameof(GetRestaurantById), new { id }, null);

    }

    [HttpPatch]
    [Route("{id}")]
    public async Task<IActionResult> UpdateRestaurant([FromRoute] int id, UpdateRestaurantCommand command)
    {
        command.Id = id;

        bool isRestaurantUpdated = await _mediator.Send(command);

        if (isRestaurantUpdated)
        {
            return NoContent();
        }
        else
        {
            return NotFound();
        }

    }
}
