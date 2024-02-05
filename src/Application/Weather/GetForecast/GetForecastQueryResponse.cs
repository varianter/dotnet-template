namespace Application.Weather.GetForecast;

public record GetForecastQueryResponse(DateOnly Date, int TemperatureC, string? Summary);