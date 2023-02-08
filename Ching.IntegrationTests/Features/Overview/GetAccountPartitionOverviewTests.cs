using Microsoft.EntityFrameworkCore;
using Ching.Features.Overview;
using AccountTransactionCreate = Ching.Features.AccountTransaction.Create;

namespace Ching.IntegrationTests.Features.Overview;

[Collection(nameof(SliceFixture))]
public class GetAccountPartitionOverviewTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public GetAccountPartitionOverviewTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var account1 = await _fixture.ExecuteDbContextAsync(db => db.Accounts.Where(x => x.Name == "ACC1").SingleOrDefaultAsync());
        var account2 = await _fixture.ExecuteDbContextAsync(db => db.Accounts.Where(x => x.Name == "ACC2").SingleOrDefaultAsync());

        account1.ShouldNotBeNull();
        account2.ShouldNotBeNull();

        var part1Remaining = account1.RemainingPartition.Id;
        var part1_1 = account1.Partitions[1].Id;
        var part1_2 = account1.Partitions[2].Id;

        var part2Remaining = account2.RemainingPartition.Id;

        var seedData = new List<(int, decimal)>()
        {
            (part1Remaining, 10m),
            (part1Remaining, -5m),
            (part1Remaining, 50m),
            (part1_1, -34.5m),
            (part1_2, -10m),
            (part1_2, 15m),
            (part2Remaining, 15m),
        };

        foreach (var (accountPartitionId, amount) in seedData)
        {
            await _fixture.SendAsync(new AccountTransactionCreate.Command
            (
                accountPartitionId,
                new DateOnly(),
                amount,
                "Recipient"
            ));
        }
    }

    [Fact]
    public async Task Should_return_account_partition_overview()
    {
        var command = new GetAccountPartitionOverview.Query(1);

        var overview = await _fixture.SendAsync(command);

        var partitions = await _fixture.ExecuteDbContextAsync(db => db.AccountPartitions.Where(x => x.AccountId == 1).ToListAsync());

        overview.ShouldNotBeNull();
        overview.PartitionData.Count.ShouldBe(partitions.Count);

        overview.PartitionData[0].PartitionId.ShouldBe(partitions[0].Id);
        overview.PartitionData[0].PartitionName.ShouldBe(partitions[0].Name);
        overview.PartitionData[0].Balance.ShouldBe(10m - 5m + 50m);

        overview.PartitionData[1].PartitionId.ShouldBe(partitions[1].Id);
        overview.PartitionData[1].PartitionName.ShouldBe(partitions[1].Name);
        overview.PartitionData[1].Balance.ShouldBe(-34.5m);

        overview.PartitionData[2].PartitionId.ShouldBe(partitions[2].Id);
        overview.PartitionData[2].PartitionName.ShouldBe(partitions[2].Name);
        overview.PartitionData[2].Balance.ShouldBe(-10m + 15m);
    }
}