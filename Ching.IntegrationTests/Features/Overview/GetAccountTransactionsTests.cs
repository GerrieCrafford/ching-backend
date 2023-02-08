using Microsoft.EntityFrameworkCore;
using Ching.Features.Overview;
using AccountTransactionCreate = Ching.Features.AccountTransaction.Create;

namespace Ching.IntegrationTests.Features.Overview;

[Collection(nameof(SliceFixture))]
public class GetAccountTransactionsTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public GetAccountTransactionsTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var account1 = await _fixture.ExecuteDbContextAsync(db => db.Accounts.Where(x => x.Name == "ACC1").SingleOrDefaultAsync());
        var account2 = await _fixture.ExecuteDbContextAsync(db => db.Accounts.Where(x => x.Name == "ACC2").SingleOrDefaultAsync());

        account1.ShouldNotBeNull();
        account2.ShouldNotBeNull();

        var part1Id = account1.RemainingPartition.Id;
        var part2Id = account2.RemainingPartition.Id;

        var seedData = new List<(int, decimal, DateOnly)>()
        {
            (part1Id, -123m, new DateOnly(2023, 1, 15)),
            (part2Id, 81m, new DateOnly(2023, 1, 18)),
            (part1Id, 17.5m, new DateOnly(2023, 1, 14)),
            (part2Id, 33m, new DateOnly(2023, 2, 1)),
            (part1Id, -20m, new DateOnly(2023, 1, 18)),
            (part2Id, 74.3m, new DateOnly(2023, 1, 14)),
            (part1Id, 30m, new DateOnly(2023, 1, 16)),
            (part2Id, -56m, new DateOnly(2023, 1, 18)),
            (part1Id, 99m, new DateOnly(2023, 1, 29)),
            (part1Id, 80m, new DateOnly(2023, 2, 1)),
        };

        foreach (var (accountPartitionId, amount, date) in seedData)
        {
            await _fixture.SendAsync(new AccountTransactionCreate.Command
            (
                accountPartitionId,
                date,
                amount,
                "Recipient"
            ));
        }
    }

    [Fact]
    public async Task Should_return_account_transactions()
    {
        var command = new GetAccountTransactions.Query(1);

        var transactions = await _fixture.SendAsync(command);

        transactions.ShouldNotBeNull();
        transactions.TransactionData.Count.ShouldBe(6);

        transactions.TransactionData[0].Balance.ShouldBe(17.5m);
        transactions.TransactionData[1].Balance.ShouldBe(17.5m - 123m);
        transactions.TransactionData[2].Balance.ShouldBe(17.5m - 123m + 30m);
        transactions.TransactionData[3].Balance.ShouldBe(17.5m - 123m + 30m - 20m);
        transactions.TransactionData[4].Balance.ShouldBe(17.5m - 123m + 30m - 20m + 99m);
        transactions.TransactionData[5].Balance.ShouldBe(17.5m - 123m + 30m - 20m + 99m + 80m);

        transactions.TransactionData[0].Transaction.Amount.ShouldBe(17.5m);
        transactions.TransactionData[1].Transaction.Amount.ShouldBe(-123m);
        transactions.TransactionData[2].Transaction.Amount.ShouldBe(30m);
        transactions.TransactionData[3].Transaction.Amount.ShouldBe(-20m);
        transactions.TransactionData[4].Transaction.Amount.ShouldBe(99m);
        transactions.TransactionData[5].Transaction.Amount.ShouldBe(80m);

        transactions.TransactionData[0].Transaction.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 14));
        transactions.TransactionData[1].Transaction.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 15));
        transactions.TransactionData[2].Transaction.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 16));
        transactions.TransactionData[3].Transaction.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 18));
        transactions.TransactionData[4].Transaction.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 29));
        transactions.TransactionData[5].Transaction.Date.ShouldBeEquivalentTo(new DateOnly(2023, 2, 1));

        command = new GetAccountTransactions.Query(2);

        transactions = await _fixture.SendAsync(command);

        transactions.ShouldNotBeNull();
        transactions.TransactionData.Count.ShouldBe(4);

        transactions.TransactionData[0].Balance.ShouldBe(74.3m);
        transactions.TransactionData[1].Balance.ShouldBe(74.3m + 81m);
        transactions.TransactionData[2].Balance.ShouldBe(74.3m + 81m - 56m);
        transactions.TransactionData[3].Balance.ShouldBe(74.3m + 81m - 56m + 33m);

        transactions.TransactionData[0].Transaction.Amount.ShouldBe(74.3m);
        transactions.TransactionData[1].Transaction.Amount.ShouldBe(81m);
        transactions.TransactionData[2].Transaction.Amount.ShouldBe(-56m);
        transactions.TransactionData[3].Transaction.Amount.ShouldBe(33m);

        transactions.TransactionData[0].Transaction.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 14));
        transactions.TransactionData[1].Transaction.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 18));
        transactions.TransactionData[2].Transaction.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 18));
        transactions.TransactionData[3].Transaction.Date.ShouldBeEquivalentTo(new DateOnly(2023, 2, 1));
    }
}