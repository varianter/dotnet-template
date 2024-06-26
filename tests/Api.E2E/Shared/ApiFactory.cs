using System.Data.Common;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace Api.E2E.Shared;

/// <summary>
///     Used to create a test server for the API and setup + migrate a TestContainer-database.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime, ICollectionFixture<ApiFactory>
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithDatabase("test")
        .WithUsername("bob123")
        .WithPassword("CorrectHorseBatteryStaple")
        .WithImage("postgres:13-alpine3.19")
        .WithName("ApiFactoryTestsDb")
        .WithLabel("reuse-id", "ApiFactoryTestsDb")
        .WithReuse(true)
        .Build();

    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;
    public HttpClient HttpClient { get; private set; } = default!;
    public DatabaseContext DbContext { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        DbContext = CreateContext();
        await DbContext.Database.MigrateAsync();

        HttpClient = CreateClient();

        _dbConnection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString());
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
        return _postgreSqlContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.Configure<InfrastructureConfig>(opts =>
            {
                opts.ConnectionString = _postgreSqlContainer.GetConnectionString();
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

    private DatabaseContext CreateContext()
    {
        return new DatabaseContext(
            Options.Create(
                new InfrastructureConfig
                {
                    ConnectionString = _postgreSqlContainer.GetConnectionString()
                }));
    }
}