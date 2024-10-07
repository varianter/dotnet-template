using System.Data.Common;
using System.Diagnostics;
using Infrastructure;
using Infrastructure.TestContainers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace Api.E2E.Shared;

/// <summary>
///     Used to create a test server for the API and setup + migrate a TestContainer-database.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime, ICollectionFixture<ApiFactory>
{
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;
    public TestContainersFactory TestContainersFactory { get; private set; } = default!;
    public HttpClient HttpClient { get; private set; } = default!;
    public DatabaseContext DbContext { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        var config = new TestContainersConfig
        {
            Enabled = true,
            RunMigrations = true
        };
        
        TestContainersFactory = new TestContainersFactory(config, NullLogger<TestContainersFactory>.Instance);
        await TestContainersFactory.Start(overrides: new TestContainersOverrides
        {
            DatabaseTestContainerName = "testcontainers-api-e2e-tests-db",
            HostPort = 51235
        });
        
        DbContext = new DatabaseContext(Options.Create(new InfrastructureConfig
        {
            ConnectionString = TestContainersFactory.CurrentConnectionString ?? throw new UnreachableException(),
            EnableSensitiveDataLogging = true
        }));
        
        HttpClient = CreateClient();

        _dbConnection = new NpgsqlConnection(TestContainersFactory.CurrentConnectionString);
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"], // add your own schemas here if not using public
            TablesToIgnore =
            [
                new Table("public", "__EFMigrationsHistory"),
            ]
        });
    }

    public new Task DisposeAsync()
    {
        HttpClient.Dispose();
        return TestContainersFactory.Stop();
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config => config.AddJsonFile("appsettings.Test.json"));
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(TestContainersService));

            services.Configure<InfrastructureConfig>(opts =>
            {
                opts.ConnectionString = TestContainersFactory.CurrentConnectionString ?? throw new UnreachableException();
            });
            
            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                var openIdConnectConfiguration = new OpenIdConnectConfiguration
                {
                    Issuer = MockJwtTokensHelper.Issuer
                };
                openIdConnectConfiguration.SigningKeys.Add(MockJwtTokensHelper.SecurityKey);
                opts.Configuration = openIdConnectConfiguration;
            });
        });
    }

    /// <summary>
    ///     Used to reset the database to its original state between test runs.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }
}