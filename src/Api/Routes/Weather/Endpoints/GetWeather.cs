using Api.Features.Weather.Models;
using Api.Routes.Weather.Models;
using Application.Weather.GetForecast;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Weather.Endpoints;

public static class GetWeather
{
    public static async Task<Results<Ok<GetForecastResponse>, ProblemHttpResult>> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] GetWeatherRequest request)
    {
        var result = await mediator.Send(new GetForecastQuery { From = request.Date });

        if (result.IsFailed)
        {
            return TypedResults.Problem(new ProblemDetails());
        }

        return TypedResults.Ok(result.Value);
    }
}