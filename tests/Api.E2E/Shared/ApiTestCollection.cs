namespace Api.E2E.Shared;

[CollectionDefinition(CollectionName)]
public class ApiTestCollection : ICollectionFixture<ApiFactory>
{
    public const string CollectionName = nameof(ApiTestCollection);
}