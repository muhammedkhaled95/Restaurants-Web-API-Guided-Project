using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Restaurants.Application.Restaurants;
using Restaurants.Application.Restaurants.Commands.CreateRestaurants;
using Restaurants.Application.Restaurants.Commands.DeleteRestaurants;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurants;
using Restaurants.Application.Restaurants.Commands.UploadRestaurantsLogo;
using Restaurants.Application.Restaurants.DTOs;
using Restaurants.Application.Restaurants.Queries.GetAllRestaurants;
using Restaurants.Application.Restaurants.Queries.GetRestaurantById;
using Restaurants.Domain.Constants;
using Restaurants.Infrastructure.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Restaurants.API.Controllers;

[ApiController]
[Route("api/restaurants")]
[Authorize]
public class RestaurantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RestaurantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    //[Authorize(Policy = PolicyNames.CreatedAtLeastTwoRestaurants)]
    public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAll([FromQuery] GetAllRestaurantsQuery? query)
    {
        var restautants = await _mediator.Send(query!);

        return Ok(restautants);
    }

    [HttpGet]
    [Route("{id}")]
    //[Authorize(Policy = PolicyNames.HasNationality)]
    //[Authorize(Policy = PolicyNames.AtLeast20YearsOfAge)]
    public async Task<ActionResult<RestaurantDto>> GetRestaurantById([FromRoute] int id)
    {
        var restaurant = await _mediator.Send(new GetRestaurantByIdQuery(id));
        return Ok(restaurant);        
    }

    [HttpDelete]
    [Route("{id}")]
    /*
     * In ASP.NET Web API, the [ProducesResponseType] attribute is used to document the possible HTTP response types of an action method.
     * It is especially useful for Swagger (OpenAPI) documentation, where it helps describe the expected responses clearly. How It Affects Swagger Docs:

        -It adds response information to the Swagger UI.
        -It shows the HTTP status codes and their associated response types.
        -It improves API documentation, making it easier for consumers to understand what to expect.
     */
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRestaurantById([FromRoute] int id)
    {
        await _mediator.Send(new DeleteRestaurantCommand(id));
        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Owner)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRestaurant([FromRoute] int id, UpdateRestaurantCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Uploads a logo for a specific restaurant.
    /// </summary>
    /// <param name="id">The unique identifier of the restaurant.</param>
    /// <param name="file">The image file to be uploaded as the restaurant logo.</param>
    /// <returns>Returns a 204 No Content response if the upload is successful.</returns>
    /// <remarks>
    /// This endpoint allows users to upload a logo for a restaurant.
    /// The uploaded file is processed and stored as per the business logic in <see cref="UploadRestaurantLogoCommand"/>.
    /// </remarks>
    /// <response code="204">Logo uploaded successfully.</response>
    /// <response code="400">Invalid request or file is missing.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPost] // Specifies that this action handles HTTP POST requests
    [Route("{id}/logo")] // Defines the route as "/{id}/logo", meaning the request must include a restaurant ID in the URL
    public async Task<IActionResult> UploadRestaurantLogo([FromRoute] int id, IFormFile file)
    {
        // Ensure that the file is not null before proceeding
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required."); // Return a 400 response if no file is provided
        }

        // Open a read stream from the uploaded file
        using var stream = file.OpenReadStream();

        // Create a command object that will be sent to the Mediator (CQRS pattern)
        var command = new UploadRestaurantLogoCommand()
        {
            RestaurantId = id,   // Assign the provided restaurant ID
            FileName = file.FileName, // Capture the original file name
            File = stream  // Pass the file stream for processing
        };

        // Send the command to the Mediator, which will handle the logic of storing the logo
        await _mediator.Send(command);

        // Return a 204 No Content response to indicate success without returning any content
        return NoContent();
    }

}
