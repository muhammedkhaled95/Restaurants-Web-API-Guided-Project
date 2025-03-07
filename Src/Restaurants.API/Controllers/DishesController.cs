using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Dishes.Commands.CreateDishes;
using Restaurants.Application.Dishes.Commands.DeleteDishes;
using Restaurants.Application.Dishes.DTOs;
using Restaurants.Application.Dishes.Queries.GetDishByIdForRestaurant;
using Restaurants.Application.Dishes.Queries.GetDishesForRestaurant;
using System.Formats.Asn1;

namespace Restaurants.API.Controllers;

[ApiController]
[Route("api/restaurants/{restaurantId}/dishes")]
public class DishesController : ControllerBase
{
    private readonly IMediator _mediator;
    public DishesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDish([FromRoute] int restaurantId, [FromBody] CreateDishCommand command)
    {
        command.RestaurantId = restaurantId;
        var dishId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetDishByIdForRestaurant), new { restaurantId, dishId }, null);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DishDto>>> GetAllDishesForRestaurant([FromRoute] int restaurantId)
    {
        var dishes = await _mediator.Send(new GetDishesForRestaurantQuery(restaurantId));
        return Ok(dishes);
    }

    [HttpGet]
    [Route("{dishId}")]
    public async Task<ActionResult<DishDto>> GetDishByIdForRestaurant([FromRoute] int restaurantId, [FromRoute] int dishId)
    {
        var dish = await _mediator.Send(new GetDishByIdForRestaurantQuery(restaurantId, dishId));
        return Ok(dish);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteDishesForRestaurant([FromRoute] int restaurantId)
    {
        await _mediator.Send(new DeleteDishesForRestautantCommand(restaurantId));
        return NoContent();
    }
}
