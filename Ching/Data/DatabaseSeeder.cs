using Ching.Entities;

namespace Ching.Data;

public static class DatabaseSeeder
{
    public static void Seed(ChingContext db)
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

        var ba1 = new BudgetAssignmentTransaction(
            cat1,
            100m,
            new BudgetMonth(2023, 2),
            "Some note"
        );
        var ba2 = new BudgetAssignmentTransaction(
            cat2,
            500m,
            new BudgetMonth(2023, 2),
            "Some note"
        );
        db.BudgetAssignmentsTransactions.AddRange(ba1, ba2);

        db.SaveChanges();
    }
}
