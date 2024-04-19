using Domain;
using FluentResults;
using MediatR;

namespace Application.Weather.AddForecast;

internal class AddForecastCommandHandler(IWeatherRepository weatherRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<AddForecastCommand, Result>
{
    public async Task<Result> Handle(AddForecastCommand request, CancellationToken cancellationToken)
    {
        var forecast = await weatherRepository.GetForecastAsync(request.Date);
        if (forecast is not null)
            return Result.Fail(new Error("Forecast already exists"));
        
        var result = Forecast.New(request.Date, request.TemperatureC, request.Summary);

        if (result.IsFailed)
            return result.ToResult();

        try
        {
            await weatherRepository.AddForecastAsync(result.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to add forecast").CausedBy(ex));
        }
    }
}