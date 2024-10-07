using FluentResults;
using MediatR;

namespace Application.Weather.DeleteForecast;

public record DeleteForecastCommand(DateOnly Date) : IRequest<Result>;