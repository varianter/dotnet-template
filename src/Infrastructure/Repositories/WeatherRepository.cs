using Api.Features.Weather.Models;
using Application.Weather;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class WeatherRepository(DatabaseContext context) : IWeatherRepository
{
    public async Task<Forecast?> GetForecastAsync(DateOnly date)
    {
        return await context.Forecasts
            .Where(f => f.Date == date)
            .FirstOrDefaultAsync();
    }

    public async Task AddForecastAsync(Forecast forecast)
    {
        context.Forecasts.Add(forecast);
        await context.SaveChangesAsync();
    }
}