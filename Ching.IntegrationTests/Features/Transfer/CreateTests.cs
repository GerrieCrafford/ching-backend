using Ching.DTOs;
using Ching.Features.Transfer;

namespace Ching.IntegrationTests.Features.Transfer;

[Collection(nameof(SliceFixture))]
public class CreateTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public CreateTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_transfer()
    {
        var cat1 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 1");
        var part1 = await _fixture.FindAsync<Entities.AccountPartition>(x => x.Name == "ACC1P1");
        var part2 = await _fixture.FindAsync<Entities.AccountPartition>(x => x.Name == "ACC1P2");

        part1.ShouldNotBeNull();
        part2.ShouldNotBeNull();

        var command = new CreateTransfer.Command(new DateOnly(2023, 1, 20), 121.64m, part2.Id, part1.Id);
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
        var part1 = await _fixture.FindAsync<Entities.AccountPartition>(x => x.Name == "ACC1P1");
        var part2 = await _fixture.FindAsync<Entities.AccountPartition>(x => x.Name == "ACC1P2");

        cat1.ShouldNotBeNull();
        part1.ShouldNotBeNull();
        part2.ShouldNotBeNull();

        var command = new CreateSavingsPayment.Command
        (
            new DateOnly(2023, 4, 1),
            992.4m,
            part1.Id,
            part2.Id,
            new CreateSavingsPayment.Command.BudgetAssignmentData
            (
                cat1.Id,
                new BudgetMonthDTO(2023, 3),
                992.4m
            )
        );

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