using FluentResults;
using MediatR;

namespace Application.Weather.AddForecast;

public record DeleteForecastCommand(DateOnly Date) : IRequest<Result>;