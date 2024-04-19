using FluentResults;

namespace Domain;

public class Forecast
{
    public Guid Id { get; private set; }
    public DateOnly Date { get; private set; }
    public int TemperatureC { get; private set; }
    public string? Summary { get; private set; }
    public bool IsDeleted { get; set; }

    public static Result<Forecast> New(DateOnly date, int temperatureC, string? summary)
    {
        if (temperatureC is < -90 or > 60) return Result.Fail("Temperature must be between -90 and 60.");

        return new Forecast
        {
            Id = Guid.NewGuid(),
            Date = date,
            TemperatureC = temperatureC,
            Summary = summary
        };
    }
    
    public void Delete()
    {
        IsDeleted = true;
    }
}