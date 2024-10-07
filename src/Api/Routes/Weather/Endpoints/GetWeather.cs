using Api.Routes.Weather.Models;
using Application.Weather.GetForecast;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Routes.Weather.Endpoints;

public static class GetWeather
{
    public static async Task<Results<Ok<GetWeatherResponse>, ProblemHttpResult>> Handle(
        IMediator mediator,
        string date)
    {
        var result = await mediator.Send(new GetForecastQuery { From = DateOnly.ParseExact(date, "yyyy-MM-dd") });

        if (result.IsFailed)
            return TypedResults.Problem(string.Join(",", result.Errors.Select(x => x.Message)), statusCode: 500);

        var forecast = result.Value;

        var response = new GetWeatherResponse
        {
            Forecast =
                new GetWeatherResponse.ForecastPayload(
                    forecast.Date,
                    forecast.TemperatureC,
                    forecast.Summary
                )
        };

        return TypedResults.Ok(response);
    }
}