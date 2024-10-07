using FluentResults;
using MediatR;

namespace Application.Weather.DeleteForecast;

internal class DeleteForecastCommandHandler(IWeatherRepository weatherRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteForecastCommand, Result>
{
    public async Task<Result> Handle(DeleteForecastCommand request, CancellationToken cancellationToken)
    {
        var forecast = await weatherRepository.GetForecastAsync(request.Date);
        if (forecast is null)
            return Result.Ok();

        try
        {
            forecast.Delete();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to delete forecast").CausedBy(ex));
        }
    }
}