namespace Api.Routes.Weather.Models;

public record PostWeatherRequest(DateOnly Date, int TemperatureC, string? Summary); 