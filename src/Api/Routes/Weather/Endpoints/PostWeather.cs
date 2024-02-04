using Api.Routes.Weather.Models;
using Application.Weather.AddForecast;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Weather.Endpoints;

public static class PostWeather
{
    public static async Task<Results<Created, ProblemHttpResult>> Handle(
        [FromServices] IMediator mediator,
        PostWeatherRequest request)
    {
        var result = await mediator.Send(new AddForecastCommand(request.Date, request.TemperatureC, request.Summary));

        if (result.IsFailed)
        {
            return TypedResults.Problem(new ProblemDetails());
        }

        return TypedResults.Created();
    }
}