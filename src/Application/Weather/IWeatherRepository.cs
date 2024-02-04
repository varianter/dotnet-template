using Api.Features.Weather.Models;
using Domain;
using FluentResults;

namespace Application.Weather;

public interface IWeatherRepository
{
    Task<Forecast?> GetForecastAsync(DateOnly date);
    Task AddForecastAsync(Forecast resultValue);
}