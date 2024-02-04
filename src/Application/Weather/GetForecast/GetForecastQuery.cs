using Api.Features.Weather.Models;
using FluentResults;
using MediatR;

namespace Application.Weather.GetForecast;

public record GetForecastQuery : IRequest<Result<GetForecastResponse>>
{
    public DateOnly From { get; set; } 
}