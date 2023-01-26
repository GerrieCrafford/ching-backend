using Ching.Features.Overview;
using AccountTransactionCreate = Ching.Features.AccountTransaction.CreateFromBudgetAssignments;
using TransferCreateSavingsPayment = Ching.Features.Transfer.CreateSavingsPayment;
using BudgetIncreaseCreate = Ching.Features.BudgetIncrease.Create;
using Microsoft.EntityFrameworkCore;

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
        {
            Date = new DateOnly(),
            AccountPartitionId = 1,
            BudgetAssignments = new List<AccountTransactionCreate.Command.BudgetAssignment>
            {
                new AccountTransactionCreate.Command.BudgetAssignment { Amount = 112m, BudgetCategoryId = 1, BudgetMonth = new Entities.BudgetMonth(2023, 1)},
                new AccountTransactionCreate.Command.BudgetAssignment { Amount = 52m, BudgetCategoryId = 2, BudgetMonth = new Entities.BudgetMonth(2023, 1)},
                new AccountTransactionCreate.Command.BudgetAssignment { Amount = 332m, BudgetCategoryId = 1, BudgetMonth = new Entities.BudgetMonth(2023, 2)},
            }
        });

        var accountTransactionId2 = await _fixture.SendAsync(new AccountTransactionCreate.Command
        {
            Date = new DateOnly(),
            AccountPartitionId = 1,
            BudgetAssignments = new List<AccountTransactionCreate.Command.BudgetAssignment>
            {
                new AccountTransactionCreate.Command.BudgetAssignment { Amount = 982m, BudgetCategoryId = 2, BudgetMonth = new Entities.BudgetMonth(2023, 1)},
                new AccountTransactionCreate.Command.BudgetAssignment { Amount = 33m, BudgetCategoryId = 3, BudgetMonth = new Entities.BudgetMonth(2023, 1)},
            }
        });

        var spTransferId = await _fixture.SendAsync(new TransferCreateSavingsPayment.Command
        {
            Amount = 921m,
            Date = new DateOnly(),
            SourcePartitionId = 1,
            DestinationPartitionId = 2,
            BudgetAssignment = new TransferCreateSavingsPayment.Command.BudgetAssignmentData
            {
                Amount = 921m,
                BudgetCategoryId = 3,
                BudgetMonth = new TransferCreateSavingsPayment.Command.BudgetMonthData { Year = 2023, Month = 1 }
            }
        });

        var biTransferId1 = await _fixture.SendAsync(new BudgetIncreaseCreate.Command
        {
            BudgetCategoryId = 1,
            BudgetMonth = new BudgetIncreaseCreate.Command.BudgetMonthData { Year = 2023, Month = 1 },
            Transfer = new BudgetIncreaseCreate.Command.TransferData
            {
                Amount = IncreaseAmount,
                Date = new DateOnly(),
                SourcePartitionId = 1,
                DestinationPartitionId = 2
            }
        });

        var biTransferId2 = await _fixture.SendAsync(new BudgetIncreaseCreate.Command
        {
            BudgetCategoryId = 2,
            BudgetMonth = new BudgetIncreaseCreate.Command.BudgetMonthData { Year = 2023, Month = 2 },
            Transfer = new BudgetIncreaseCreate.Command.TransferData
            {
                Amount = 442m,
                Date = new DateOnly(),
                SourcePartitionId = 1,
                DestinationPartitionId = 2
            }
        });
    }

    [Fact]
    public async Task Should_return_overview()
    {
        var command = new GetBudgetOverview.Query { Month = 1, Year = 2023 };

        var overview = await _fixture.SendAsync(command);

        overview.ShouldNotBeNull();

        overview.OverviewItems.Count.ShouldBe(3);

        overview.OverviewItems[0].CategoryName.ShouldBe("Seed category 1");
        overview.OverviewItems[1].CategoryName.ShouldBe("Seed category 2");
        overview.OverviewItems[2].CategoryName.ShouldBe("Seed category 3");

        overview.OverviewItems[0].Available.ShouldBe(113 + IncreaseAmount);
        overview.OverviewItems[1].Available.ShouldBe(403);
        overview.OverviewItems[2].Available.ShouldBe(551);

        overview.OverviewItems[0].Spent.ShouldBe(112m);
        overview.OverviewItems[1].Spent.ShouldBe(52m + 982m);
        overview.OverviewItems[2].Spent.ShouldBe(33m + 921m);
    }
}