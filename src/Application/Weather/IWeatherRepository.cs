using Domain;

namespace Application.Weather;

public interface IWeatherRepository
{
    Task<Forecast?> GetForecastAsync(DateOnly date);
    Task AddForecastAsync(Forecast resultValue);
}