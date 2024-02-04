using Api.Features.Weather.Models;
using FluentResults;
using MediatR;

namespace Application.Weather.GetForecast;

internal class GetForecastQueryHandler(IWeatherRepository weatherRepository) : IRequestHandler<GetForecastQuery, Result<GetForecastResponse>>
{
    public async Task<Result<GetForecastResponse>> Handle(GetForecastQuery request, CancellationToken cancellationToken)
    {
        try {
            var forecast = await weatherRepository.GetForecastAsync(request.From);
            return new GetForecastResponse(forecast.Date, forecast.TemperatureC, forecast.Summary);
        } catch (Exception ex) {
            return Result.Fail(new Error("Failed to get forecast.").CausedBy(ex));
        }
    }
}