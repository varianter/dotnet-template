using Api.Authorization;
using Api.Routes.Weather.Endpoints;

namespace Api.Routes.Weather;

public static class WeatherUserGroup
{
    public static WebApplication MapWeatherUserGroup(this WebApplication app)
    {
        var group = app.MapUserGroup("weather");

        group.MapGet("/{date}", GetWeather.Handle);
        group.MapPost("/", PostWeather.Handle)
            .RequireAuthorization(AuthorizationPolicy.Write);

        return app;
    }
}