using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace Infrastructure.TestContainers;

public class TestContainersFactory(TestContainersConfig config, ILogger<TestContainersFactory> logger)
{
    private const string DatabaseName = "test";
    private const string Username = "test";
    private const string Password = "test123###!";
    private const int HostPort = 51234;

    public static readonly string DefaultDbConnectionString =
        $"Host=127.0.0.1;Port={HostPort};Database={DatabaseName};Username={Username};Password={Password}";
    
    private PostgreSqlContainer? _postgreSqlContainer;

    public string? CurrentConnectionString => _postgreSqlContainer?.GetConnectionString();

    public async Task Start(CancellationToken cancellationToken = default, TestContainersOverrides? overrides = null)
    {
        try
        {
            if (config.Enabled)
            {
                var hostPort = overrides?.HostPort ?? HostPort;
                var databaseTestContainerName = overrides?.DatabaseTestContainerName ?? "testcontainers-api-template-db";

                logger.LogInformation("Starting TestContainers");
                _postgreSqlContainer = new PostgreSqlBuilder()
                    .WithName(databaseTestContainerName)
                    .WithReuse(true)
                    .WithDatabase(DatabaseName)
                    .WithUsername(Username)
                    .WithPassword(Password)
                    .WithPortBinding(hostPort, 5432)
                    .Build();

                await _postgreSqlContainer.StartAsync(cancellationToken);
                
                if (config.RunMigrations)
                {
                    var options = Options.Create(new InfrastructureConfig
                    {
                        ConnectionString = _postgreSqlContainer.GetConnectionString(),
                        EnableSensitiveDataLogging = true
                    });

                    await using var context = new DatabaseContext(options);

                    var retries = 0;
                    while (!await context.Database.CanConnectAsync(cancellationToken) && retries < 10)
                    {
                        retries++;
                        await Task.Delay(1000, cancellationToken);
                    }

                    logger.LogInformation("Running database migrations");
                    await context.Database.MigrateAsync(cancellationToken); 
                }
            }
        } catch (Exception ex)
        {
            logger.LogError(ex, "Error starting TestContainers");
        }
    }

    public Task Stop(CancellationToken cancellationToken = default)
    {
        var stopTask = _postgreSqlContainer?.StopAsync(cancellationToken) ?? Task.CompletedTask;
        return stopTask;
    }

}

public class TestContainersOverrides
{
    public string? DatabaseTestContainerName { get; set; }
    public int? HostPort { get; set; }
}