using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.TestContainers;

public record TestContainersConfig
{
    public const string SectionName = "TestContainers";
    
    public required bool Enabled { get; set; }
    public required bool RunMigrations { get; set; }
}

internal static class TestContainersConfigExtensions
{
    internal static IHostApplicationBuilder AddTestContainersConfig(this IHostApplicationBuilder builder, out TestContainersConfig currentConfig)
    {
        var configurationSection = builder.Configuration.GetSection(TestContainersConfig.SectionName);
        builder.Services.Configure<TestContainersConfig>(configurationSection);
        currentConfig = configurationSection.Get<TestContainersConfig>()!;
        return builder;
    }
}