using Application.Weather;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var configSection = builder.Configuration.GetSection(nameof(InfrastructureConfig));
        builder.Services.Configure<InfrastructureConfig>(configSection);

        builder.Services.AddDbContext<DatabaseContext>();
        builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();

        return builder;
    }
}