using Ching.Features.Transfer;

namespace Ching.IntegrationTests.Features.Transfer;

[Collection(nameof(SliceFixture))]
public class CreateTests
{
    private readonly SliceFixture _fixture;
    public CreateTests(SliceFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_transfer()
    {
        var cat1 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 1");
        var part1 = await _fixture.FindAsync<Entities.AccountPartition>(x => x.Name == "Seed partition 1");
        var part2 = await _fixture.FindAsync<Entities.AccountPartition>(x => x.Name == "Seed partition 2");

        var command = new CreateTransfer.Command
        {
            Amount = 121.64m,
            SourcePartitionId = part2.Id,
            DestinationPartitionId = part1.Id,
            Date = new DateOnly(2023, 1, 20),
        };
        var transferId = await _fixture.SendAsync(command);

        var transfer = await _fixture.GetLast<Entities.Transfer>();

        transfer.Id.ShouldBe(transferId);
        transfer.Amount.ShouldBe(121.64m);
        transfer.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 20));
        transfer.SourcePartition.Id.ShouldBe(part2.Id);
        transfer.DestinationPartition.Id.ShouldBe(part1.Id);
        transfer.BudgetAssignment.ShouldBeNull();
    }

    [Fact]
    public async Task Should_create_savings_payment()
    {
        var cat1 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 1");
        var part1 = await _fixture.FindAsync<Entities.AccountPartition>(x => x.Name == "Seed partition 1");
        var part2 = await _fixture.FindAsync<Entities.AccountPartition>(x => x.Name == "Seed partition 2");

        var command = new CreateSavingsPayment.Command
        {
            Date = new DateOnly(2023, 4, 1),
            Amount = 992.4m,
            SourcePartitionId = part1.Id,
            DestinationPartitionId = part2.Id,
            BudgetAssignment = new CreateSavingsPayment.Command.BudgetAssignmentData
            {
                Amount = 992.4m,
                BudgetCategoryId = cat1.Id,
                BudgetMonth = new CreateSavingsPayment.Command.BudgetMonthData
                {
                    Year = 2023,
                    Month = 3
                }
            }
        };

        var transferId = await _fixture.SendAsync(command);

        var transfer = await _fixture.GetLast<Entities.Transfer>();

        transfer.Id.ShouldBe(transferId);
        transfer.Amount.ShouldBe(992.4m);
        transfer.Date.ShouldBeEquivalentTo(new DateOnly(2023, 4, 1));
        transfer.SourcePartition.Id.ShouldBe(part1.Id);
        transfer.DestinationPartition.Id.ShouldBe(part2.Id);
        transfer.BudgetAssignment.ShouldNotBeNull();
        transfer.BudgetAssignment.Amount.ShouldBe(992.4m);
        transfer.BudgetAssignment.BudgetCategory.Id.ShouldBe(cat1.Id);
        transfer.BudgetAssignment.BudgetMonth.Month.ShouldBe(3);
        transfer.BudgetAssignment.BudgetMonth.Year.ShouldBe(2023);
    }
}