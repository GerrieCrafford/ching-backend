using Ching.Features.Overview;
using AccountTransactionCreate = Ching.Features.AccountTransaction.CreateFromBudgetAssignments;
using TransferCreateSavingsPayment = Ching.Features.Transfer.CreateSavingsPayment;
using BudgetIncreaseCreate = Ching.Features.BudgetIncrease.Create;
using Microsoft.EntityFrameworkCore;
using Ching.DTOs;

namespace Ching.IntegrationTests.Features.Overview;

[Collection(nameof(SliceFixture))]
public class GetBudgetOverviewTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public GetBudgetOverviewTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    static decimal IncreaseAmount { get; set; }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        IncreaseAmount = 132m;

        var accountTransactionId1 = await _fixture.SendAsync(new AccountTransactionCreate.Command
        (
            1,
            new DateOnly(),
            "Recipient",
            new List<AccountTransactionCreate.Command.BudgetAssignment>
            {
                new AccountTransactionCreate.Command.BudgetAssignment(1, new BudgetMonthDTO(2023, 1), 112m),
                new AccountTransactionCreate.Command.BudgetAssignment(2, new BudgetMonthDTO(2023, 1), 52m),
                new AccountTransactionCreate.Command.BudgetAssignment(1, new BudgetMonthDTO(2023, 2), 332m),
            }
        ));

        var accountTransactionId2 = await _fixture.SendAsync(new AccountTransactionCreate.Command
        (
            1,
            new DateOnly(),
            "Recipient",
            new List<AccountTransactionCreate.Command.BudgetAssignment>
            {
                new AccountTransactionCreate.Command.BudgetAssignment(2, new BudgetMonthDTO(2023, 1), 982m),
                new AccountTransactionCreate.Command.BudgetAssignment(3, new BudgetMonthDTO(2023, 1), 33m),
            }
        ));

        var spTransferId = await _fixture.SendAsync(new TransferCreateSavingsPayment.Command
        (
            new DateOnly(),
            921m,
            1,
            2,
            new TransferCreateSavingsPayment.Command.BudgetAssignmentData(3, new BudgetMonthDTO(2023, 1), 921m)
        ));

        var biTransferId1 = await _fixture.SendAsync(new BudgetIncreaseCreate.Command
        (
            1,
            new BudgetMonthDTO(2023, 1),
            new BudgetIncreaseCreate.Command.TransferData(new DateOnly(), IncreaseAmount, 1, 2)
        ));

        var biTransferId2 = await _fixture.SendAsync(new BudgetIncreaseCreate.Command
        (
            2,
            new BudgetMonthDTO(2023, 2),
            new BudgetIncreaseCreate.Command.TransferData(new DateOnly(), 442m, 1, 2)
        ));
    }

    [Fact]
    public async Task Should_return_overview()
    {
        var command = new GetBudgetOverview.Query(2023, 1);

        var overview = await _fixture.SendAsync(command);

        overview.ShouldNotBeNull();

        overview.OverviewItems.Count.ShouldBe(4);

        overview.OverviewItems[0].CategoryName.ShouldBe("Seed category 1");
        overview.OverviewItems[1].CategoryName.ShouldBe("Seed category 2");
        overview.OverviewItems[2].CategoryName.ShouldBe("Seed category 3");
        overview.OverviewItems[3].CategoryName.ShouldBe("Seed category 4");

        overview.OverviewItems[0].Available.ShouldBe(113 + IncreaseAmount);
        overview.OverviewItems[1].Available.ShouldBe(403);
        overview.OverviewItems[2].Available.ShouldBe(551);
        overview.OverviewItems[3].Available.ShouldBe(91);

        overview.OverviewItems[0].Spent.ShouldBe(112m);
        overview.OverviewItems[1].Spent.ShouldBe(52m + 982m);
        overview.OverviewItems[2].Spent.ShouldBe(33m + 921m);
        overview.OverviewItems[3].Spent.ShouldBe(0m);

        overview.OverviewItems[0].ParentCategoryId.ShouldBeNull();
        overview.OverviewItems[1].ParentCategoryId.ShouldBeNull();
        overview.OverviewItems[2].ParentCategoryId.ShouldBeNull();
        overview.OverviewItems[3].ParentCategoryId.ShouldBe(3);
    }
}