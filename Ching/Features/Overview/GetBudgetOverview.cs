namespace Ching.Features.Overview;

using Microsoft.EntityFrameworkCore;
using MediatR;
using Ching.Data;

public class GetBudgetOverview
{
    public record Query : IRequest<Result>
    {
        public int Year { get; init; }
        public int Month { get; init; }
    }

    public record Result
    {
        public List<BudgetOverviewItem> OverviewItems { get; init; }

        public record BudgetOverviewItem
        {
            public int CategoryId { get; init; }
            public string CategoryName { get; init; }
            public decimal Spent { get; init; }
            public decimal Available { get; init; }
        }
    }

    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var monthBudgets = await _db.MonthBudgets.Where(x => x.BudgetMonth.Year == request.Year && x.BudgetMonth.Month == request.Month).ToListAsync();

            var overviewItems = new List<Result.BudgetOverviewItem>();

            foreach (var monthBudget in monthBudgets)
            {
                var increases = await _db.BudgetIncreases.Where(x => x.BudgetCategory == monthBudget.BudgetCategory && x.BudgetMonth.Year == monthBudget.BudgetMonth.Year && x.BudgetMonth.Month == monthBudget.BudgetMonth.Month).ToListAsync();
                var increasesSum = increases.Sum(x => x.Transfer.Amount);
                var available = monthBudget.Amount + increasesSum;

                var transfers = await _db.BudgetAssignmentsTransfers.Where(x => x.BudgetCategory == monthBudget.BudgetCategory && x.BudgetMonth.Year == monthBudget.BudgetMonth.Year && x.BudgetMonth.Month == monthBudget.BudgetMonth.Month).ToListAsync();
                var transfersSpent = transfers.Sum(x => x.Amount);
                var transactions = await _db.BudgetAssignmentsTransactions.Where(x => x.BudgetCategory == monthBudget.BudgetCategory && x.BudgetMonth.Year == monthBudget.BudgetMonth.Year && x.BudgetMonth.Month == monthBudget.BudgetMonth.Month).ToListAsync();
                var transactionsSpent = transactions.Sum(x => x.Amount);

                overviewItems.Add(new Result.BudgetOverviewItem
                {
                    Spent = transfersSpent + transactionsSpent,
                    Available = available,
                    CategoryId = monthBudget.BudgetCategory.Id,
                    CategoryName = monthBudget.BudgetCategory.Name,
                });
            }

            return new Result
            {
                OverviewItems = overviewItems
            };
        }
    }
}