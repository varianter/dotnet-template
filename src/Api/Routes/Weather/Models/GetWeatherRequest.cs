using FluentValidation;

namespace Api.Routes.Weather.Models;

public record GetWeatherRequest
{
    public DateOnly Date { get; set; }
}

internal class GetWeatherRequestValidator : AbstractValidator<GetWeatherRequest>
{
    public GetWeatherRequestValidator()
    {
        RuleFor(x => x.Date).NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Date must be greater than or equal to today's date");
    }
}