using Api.Routes.Weather.Models;
using Application.Weather.AddForecast;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Weather.Endpoints;

public class DeleteWeather
{
    public static async Task<Results<NoContent, ProblemHttpResult, ValidationProblem>> Handle(
        IMediator mediator,
        string date)
    {
        var result = await mediator.Send(new DeleteForecastCommand(DateOnly.ParseExact(date, "yyyy-MM-dd")));

        if (result.IsFailed)
            return TypedResults.Problem(string.Join(",", result.Errors.Select(x => x.Message)), statusCode: 500);

        return TypedResults.NoContent();
    }
}