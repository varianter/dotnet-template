using Api.Routes.Weather.Models;
using Application.Weather.AddForecast;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes.Weather.Endpoints;

public static class PostWeather
{
    public static async Task<Results<Created, ProblemHttpResult, ValidationProblem>> Handle(
        [FromServices] IMediator mediator,
        [FromServices] PostWeatherRequestValidator validator,
        PostWeatherRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid) return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var result = await mediator.Send(new AddForecastCommand(request.Date, request.TemperatureC, request.Summary));

        if (result.IsFailed)
            return TypedResults.Problem(string.Join(",", result.Errors.Select(x => x.Message)), statusCode: 500);

        return TypedResults.Created();
    }
}