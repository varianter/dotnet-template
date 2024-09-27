namespace Api.Routes.Weather.Models;

// Prefer explicit request and response models which are mapped from internal models
// This allows for the internal models to change without affecting the API contract and keeps the API stable
public record GetWeatherResponse()
{
    public ForecastPayload Forecast { get; set; }

    public record ForecastPayload(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}