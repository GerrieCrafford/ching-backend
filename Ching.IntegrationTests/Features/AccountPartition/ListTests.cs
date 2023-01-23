using Ching.Features.AccountPartition;

namespace Ching.IntegrationTests.Features.AccountPartition;

[Collection(nameof(SliceFixture))]
public class ListTests : BaseTest
{
    private readonly SliceFixture _fixture;

    public ListTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_return_account_partitions()
    {
        var query = new List.Query { AccountId = 1 };

        var result = await _fixture.SendAsync(query);

        result.ShouldNotBeNull();
        result.AccountPartitions.Select(x => x.Name).ShouldBe(new List<string> { "Remaining", "ACC1P1", "ACC1P2" });

        query = new List.Query { AccountId = 2 };

        result = await _fixture.SendAsync(query);

        result.ShouldNotBeNull();
        result.AccountPartitions.Select(x => x.Name).ShouldBe(new List<string> { "Remaining", "ACC2P1", "ACC2P2" });
    }
}