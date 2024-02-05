using Api.E2E.Shared;
using Infrastructure;

namespace Api.E2E;

[Collection(ApiTestCollection.CollectionName)]
public class TestsBase : IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;

    protected readonly HttpClient Client;
    protected readonly DatabaseContext DatabaseContext;

    protected TestsBase(ApiFactory apiFactory)
    {
        Client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabaseAsync;
        DatabaseContext = apiFactory.DbContext;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        // Clear ChangeTracker so data is not cached between tests
        DatabaseContext.ChangeTracker.Clear();
        return _resetDatabase();
    }
}