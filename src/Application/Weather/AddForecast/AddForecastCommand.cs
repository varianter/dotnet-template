using FluentResults;
using MediatR;

namespace Application.Weather.AddForecast;

public record AddForecastCommand(DateOnly Date, int TemperatureC, string? Summary) : IRequest<Result>;