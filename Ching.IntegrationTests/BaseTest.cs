namespace Ching.IntegrationTests;

public class BaseTest : IAsyncLifetime
{
    private readonly SliceFixture _fixture;
    public BaseTest(SliceFixture fixture) => _fixture = fixture;
    public Task DisposeAsync()
    {
        return _fixture.ExecuteDbContextAsync(db => _fixture.ClearDatabase(db));
    }

    public Task InitializeAsync()
    {
        return _fixture.ExecuteDbContextAsync(db => _fixture.SeedDatabase(db));
    }
}