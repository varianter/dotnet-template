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
        string date)
    {
        var result = await mediator.Send(new GetForecastQuery { From = DateOnly.ParseExact(date, "yyyy-MM-dd") });

        if (result.IsFailed)
        {
            return TypedResults.Problem(string.Join(",", result.Errors.Select(x => x.Message)), statusCode: 500);
        }

        return TypedResults.Ok(result.Value);
    }
}