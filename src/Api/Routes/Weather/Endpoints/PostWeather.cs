using Api.Routes.Weather.Models;
using Application.Weather.AddForecast;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Weather.Endpoints;

public static class PostWeather
{
    public static async Task<Results<Created, ProblemHttpResult, ValidationProblem>> Handle(
        IMediator mediator,
        PostWeatherRequestValidator validator,
        [FromBody] PostWeatherRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid) return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var dateOnly = new DateOnly(request.Date.Year, request.Date.Month, request.Date.Day);
        var result = await mediator.Send(new AddForecastCommand(dateOnly, request.TemperatureC, request.Summary));

        if (result.IsFailed)
            return TypedResults.Problem(string.Join(",", result.Errors.Select(x => x.Message)), statusCode: 500);

        return TypedResults.Created();
    }
}