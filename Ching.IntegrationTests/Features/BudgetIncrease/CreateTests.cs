using Microsoft.EntityFrameworkCore;
using Ching.Features.BudgetIncrease;

namespace Ching.IntegrationTests.Features.BudgetIncrease;

[Collection(nameof(SliceFixture))]
public class CreateTests
{
    private readonly SliceFixture _fixture;
    public CreateTests(SliceFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_budget_increase()
    {
        var account = await _fixture.FindAsync<Entities.Account>(a => a.Name == "ACC1");
        var partition1 = await _fixture.FindAsync<Entities.AccountPartition>(p => p.Name == "ACC1P1");
        var partition2 = await _fixture.FindAsync<Entities.AccountPartition>(p => p.Name == "ACC1P2");
        var cat1 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 1");
        var cat2 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 2");

        var command = new Create.Command
        {
            BudgetCategoryId = cat1.Id,
            BudgetMonth = new Create.Command.BudgetMonthData
            {
                Month = 2,
                Year = 2023
            },
            Transfer = new Create.Command.TransferData
            {
                Amount = 130m,
                Date = new DateOnly(2023, 2, 14),
                SourcePartitionId = partition1.Id,
                DestinationPartitionId = partition2.Id,
            }
        };
        await _fixture.SendAsync(command);

        var transfer = await _fixture.GetLast<Entities.Transfer>();

        transfer.Amount.ShouldBe(130m);
        transfer.Date.ShouldBeEquivalentTo(new DateOnly(2023, 2, 14));
        transfer.SourcePartition.Id.ShouldBe(partition1.Id);
        transfer.DestinationPartition.Id.ShouldBe(partition2.Id);

        var increase = await _fixture.GetLast<Entities.BudgetIncrease>();

        increase.BudgetCategory.Id.ShouldBe(cat1.Id);
        increase.Transfer.Id.ShouldBe(transfer.Id);
        increase.BudgetMonth.ShouldBeEquivalentTo(new Entities.BudgetMonth(2023, 2));
    }
}