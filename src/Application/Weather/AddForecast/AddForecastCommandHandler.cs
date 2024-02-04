using Domain;
using FluentResults;
using MediatR;

namespace Application.Weather.AddForecast;

internal class AddForecastCommandHandler(IWeatherRepository weatherRepository) : IRequestHandler<AddForecastCommand, Result>
{
    public async Task<Result> Handle(AddForecastCommand request, CancellationToken cancellationToken)
    {
        var result = Forecast.New(request.Date, request.TemperatureC, request.Summary);

        if (result.IsFailed)
            return result.ToResult();
        
        try 
        {
            await weatherRepository.AddForecastAsync(result.Value);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to add forecast").CausedBy(ex));
        }
    }
}