namespace Api.E2E.Shared;

/// <summary>
///     Collection fixture for the API tests to share the same test server and database - improves run-time for tests.
/// </summary>
[CollectionDefinition(CollectionName)]
public class ApiTestCollection : ICollectionFixture<ApiFactory>
{
    public const string CollectionName = nameof(ApiTestCollection);
}