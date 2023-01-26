using Microsoft.EntityFrameworkCore;
using Ching.Features.Settlement;

namespace Ching.IntegrationTests.Features.Settlement;

[Collection(nameof(SliceFixture))]
public class CreateTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public CreateTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_settlement()
    {
        var (acc1part1, acc1part2, acc2part1, partSource, accountTransactionIds) = await _fixture.ExecuteDbContextAsync(async db =>
        {
            var acc1part1 = await db.AccountPartitions.Where(x => x.Name == "ACC1P1").FirstOrDefaultAsync();
            var acc1part2 = await db.AccountPartitions.Where(x => x.Name == "ACC1P2").FirstOrDefaultAsync();
            var acc2part1 = await db.AccountPartitions.Where(x => x.Name == "ACC2P1").FirstOrDefaultAsync();

            var partSource = await db.AccountPartitions.Where(x => x.Name == "ACC3P1").FirstOrDefaultAsync();

            var transaction1 = new Entities.AccountTransaction(new DateOnly(2023, 1, 1), 101.1m, acc1part1.Account, acc1part1, "Recipient 1");
            var transaction2 = new Entities.AccountTransaction(new DateOnly(2023, 1, 1), 202.2m, acc1part2.Account, acc1part2, "Recipient 2");
            var transaction3 = new Entities.AccountTransaction(new DateOnly(2023, 1, 1), 303.3m, acc2part1.Account, acc2part1, "Recipient 3");
            db.AccountTransactions.Add(transaction1);
            db.AccountTransactions.Add(transaction2);
            db.AccountTransactions.Add(transaction3);
            await db.SaveChangesAsync();

            return (acc1part1, acc1part2, acc2part1, partSource, new List<int> { transaction1.Id, transaction2.Id, transaction3.Id });
        });

        var command = new Create.Command
        {
            AccountTransactionIds = accountTransactionIds,
            Date = new DateOnly(2023, 1, 15),
            SourcePartitionId = partSource.Id,
        };

        await _fixture.SendAsync(command);

        // Should create two settlements, one for each account in which account
        // transactions have been specified

        var (settlement2, settlement1) = await _fixture.ExecuteDbContextAsync(async db =>
        {
            var settlements = await db.Settlements.OrderByDescending(x => x.Id).ToListAsync();
            return (settlements[0], settlements[1]);
        });

        settlement1.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 15));
        settlement2.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 15));

        settlement1.Transfer.ShouldNotBeNull();
        settlement2.Transfer.ShouldNotBeNull();

        settlement1.Transfer.Amount.ShouldBe(101.1m + 202.2m);
        settlement2.Transfer.Amount.ShouldBe(303.3m);

        settlement1.Transfer.BudgetAssignment.ShouldBeNull();
        settlement2.Transfer.BudgetAssignment.ShouldBeNull();

        settlement1.Transfer.SourcePartition.Id.ShouldBe(partSource.Id);
        settlement2.Transfer.SourcePartition.Id.ShouldBe(partSource.Id);

        settlement1.Transfer.DestinationPartition.Id.ShouldBe(acc1part1.Account.RemainingPartition.Id);
        settlement2.Transfer.DestinationPartition.Id.ShouldBe(acc2part1.Account.RemainingPartition.Id);
    }
}