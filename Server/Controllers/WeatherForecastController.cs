using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherStation.Shared;

namespace WeatherStation.Server.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private static readonly List<WeatherForecast> forecasts = new();

    static WeatherForecastController()
    {
        forecasts.AddRange(Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }));
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> GetForecasts()
    {
        return forecasts;
    }

    [HttpPost]
    [Authorize(Roles ="Administrator")]
    public WeatherForecast CreateForecast(WeatherForecast forecast)
    {
        forecasts.Add(forecast);
        return forecast;
    }
}
