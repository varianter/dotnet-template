using Api.Authorization;
using Api.Routes.Weather.Endpoints;

namespace Api.Routes.Weather;

public static class WeatherGroup
{
    public static WebApplication MapWeatherGroup(this WebApplication app)
    {
        var group = app.MapGroup("weather");
        
        group.MapGet("/{date}", GetWeather.Handle)
            .RequireAuthorization(AuthorizationPolicy.Read);
        group.MapPost("/", PostWeather.Handle)
            .RequireAuthorization(AuthorizationPolicy.Write);

        return app;
    }
}