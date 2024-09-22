using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

// Get some built-in functionality with [ApiController]
[ApiController]
[Route("[controller]")] // Gets name of class preceding "Controller" and assigns it to the Route of this controller
public class WeatherForecastController : ControllerBase
{
    private readonly string[] _summaries = // Collection expression:
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    // Using empty string route template makes path name of class prefix (before "Controller); Anything else will
    //  add to the route, ie, "extra_path" would make the get endpoint /WeatherForecast/extra_path
    [HttpGet("", Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> GetFiveDayForecast()
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            _summaries[Random.Shared.Next(_summaries.Length)]
        ))
        .ToArray();
        return forecast;
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
