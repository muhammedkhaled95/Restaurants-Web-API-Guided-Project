using Microsoft.AspNetCore.Mvc;
namespace Restaurants.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherForecastService _weatherForecastService;
    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService weatherForecastService)
    {
        _logger = logger;
        _weatherForecastService = weatherForecastService;
    }


    [HttpPost]
    [Route("/generate")]
    public IActionResult Generate([FromQuery] int numOfResults, [FromBody] Dictionary<string, int> Range)
    {
        if (numOfResults < 0 || Range["Min"] > Range["Max"]) return BadRequest();

        var result = _weatherForecastService.Get(numOfResults, Range["Min"], Range["Max"]);
        return Ok(result);
    }


    [HttpPost]
    [Route("welcome")]
    public string helloUser([FromBody] string name)
    {
        return $"Hello {name}";
    }

}
