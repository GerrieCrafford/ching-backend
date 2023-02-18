using Ching.DTOs;
using Ching.Entities;
using MediatR;
using CreateFromBudgetAssignmentsCmd = Ching.Features.AccountTransaction.CreateFromBudgetAssignments.Command;
using CreateSavingsPaymentCmd = Ching.Features.Transfer.CreateSavingsPayment.Command;

namespace Ching.Data;

public static class DatabaseSeeder
{
    public static async Task Seed(ChingContext db, IMediator mediator)
    {
        var acc1 = new Account("Cheque seed");
        var acc2 = new Account("Credit seed");
        var acc3 = new Account("Savings seed");
        db.Accounts.AddRange(acc1, acc2, acc3);

        var acc3part1 = new AccountPartition("Trip");
        var acc3part2 = new AccountPartition("Emergency");
        acc3.Partitions.Add(acc3part1);
        acc3.Partitions.Add(acc3part2);

        var cat1 = new BudgetCategory("Food");
        var cat1child = new BudgetCategory("Snacks", cat1);
        var cat2 = new BudgetCategory("Cars");
        var cat3 = new BudgetCategory("Pension");
        db.BudgetCategories.AddRange(cat1, cat2, cat3, cat1child);

        var mb1 = new MonthBudget(100m, new BudgetMonth(2023, 2), cat1);
        var mb2 = new MonthBudget(200m, new BudgetMonth(2023, 2), cat2);
        var mb3 = new MonthBudget(300m, new BudgetMonth(2023, 2), cat3);
        var mb4 = new MonthBudget(20m, new BudgetMonth(2023, 2), cat1child);
        db.MonthBudgets.AddRange(mb1, mb2, mb3, mb4);

        db.SaveChanges();

        var accountTransaction1Id = await mediator.Send(
            new CreateFromBudgetAssignmentsCmd(
                acc1.RemainingPartition.Id,
                new DateOnly(2023, 2, 3),
                "Some recipient",
                new List<CreateFromBudgetAssignmentsCmd.BudgetAssignment>
                {
                    new CreateFromBudgetAssignmentsCmd.BudgetAssignment(
                        cat1.Id,
                        new BudgetMonthDTO(2023, 2),
                        10m
                    ),
                    new CreateFromBudgetAssignmentsCmd.BudgetAssignment(
                        cat2.Id,
                        new BudgetMonthDTO(2023, 2),
                        15m
                    ),
                }
            )
        );
        var accountTransaction2Id = await mediator.Send(
            new CreateFromBudgetAssignmentsCmd(
                acc1.RemainingPartition.Id,
                new DateOnly(2023, 2, 20),
                "Some other recipient",
                new List<CreateFromBudgetAssignmentsCmd.BudgetAssignment>
                {
                    new CreateFromBudgetAssignmentsCmd.BudgetAssignment(
                        cat1.Id,
                        new BudgetMonthDTO(2023, 2),
                        30m
                    ),
                    new CreateFromBudgetAssignmentsCmd.BudgetAssignment(
                        cat2.Id,
                        new BudgetMonthDTO(2023, 2),
                        55m
                    ),
                }
            )
        );

        var transfer1Id = await mediator.Send(
            new CreateSavingsPaymentCmd(
                new DateOnly(2023, 2, 15),
                acc3part1.Id,
                acc3part2.Id,
                new CreateSavingsPaymentCmd.BudgetAssignmentData(
                    cat1.Id,
                    new BudgetMonthDTO(2023, 2),
                    150m
                )
            )
        );
    }
}
