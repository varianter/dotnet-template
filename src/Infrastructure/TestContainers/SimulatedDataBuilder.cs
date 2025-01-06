using Bogus;
using Domain;

namespace Infrastructure.TestContainers;

public class SimulatedDataBuilder
{
    public static readonly Faker<Forecast> Forecasts = new Faker<Forecast>()
        .RuleFor(f => f.Id, f => f.Random.Guid())
        .RuleFor(f => f.Date, f => f.Date.FutureDateOnly())
        .RuleFor(f => f.TemperatureC, f => f.Random.Int(-10, 40))
        .RuleFor(f => f.Summary, f => f.Lorem.Sentence())
        .RuleFor(f => f.IsDeleted, false);

    public record ForecastOptions
    {
        public int Count { get; set; }
    }

    private ForecastOptions? _forecastsOptions = null;
    

    public SimulatedDataBuilder WithForecasts(int count)
    {
        _forecastsOptions = new()
        {
            Count = count
        };
        
        return this;
    }
    
    public SimulatedDataBuilder WithAll()
    {
        return WithForecasts(10);
    }

    public SimulatedData Generate()
    {
        List<Forecast> forecasts = [];

        if (_forecastsOptions is not null)
        {
            forecasts = Forecasts.Generate(_forecastsOptions.Count);
        }

        return new()
        {
            Forecasts = forecasts
        };
    }
    
    public Task<SavedData> Save(DatabaseContext context, CancellationToken cancellationToken = default)
    {
        return Generate().Save(context, cancellationToken);
    }

    public class SimulatedData
    {
        private bool _isSaved;
        public required List<Forecast> Forecasts { get; init; }
        
        public async Task<SavedData> Save(DatabaseContext context, CancellationToken cancellationToken = default)
        {
            if (!_isSaved)
            {
                context.Forecasts.AddRange(Forecasts);

                await context.SaveChangesAsync(cancellationToken);
                context.ChangeTracker.Clear();
                
                _isSaved = true;
            }

            return new()
            {
                Forecasts = Forecasts
            };
        }
    }
    
    public class SavedData
    {
        public List<Forecast> Forecasts { get; init; }
    }
}