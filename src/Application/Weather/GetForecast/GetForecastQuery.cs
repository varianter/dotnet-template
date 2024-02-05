using FluentResults;
using MediatR;

namespace Application.Weather.GetForecast;

public record GetForecastQuery : IRequest<Result<GetForecastQueryResponse>>
{
    public DateOnly From { get; set; }
}