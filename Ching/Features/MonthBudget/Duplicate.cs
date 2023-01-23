namespace Ching.Features.MonthBudget;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using Microsoft.EntityFrameworkCore;

public class Duplicate
{
    public class Command : IRequest
    {
        public int Year { get; init; }
        public int Month { get; init; }
    }

    public class Handler : IRequestHandler<Command, Unit>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var oldMonthBudgets = await _db.MonthBudgets.Where(x => x.BudgetMonth.Year == request.Year && x.BudgetMonth.Month == request.Month).ToListAsync();

            var newBudgets = oldMonthBudgets.Select(x => new MonthBudget(x.Amount, new BudgetMonth(x.BudgetMonth), x.BudgetCategory));
            await _db.MonthBudgets.AddRangeAsync(newBudgets);
            await _db.SaveChangesAsync();

            return Unit.Value;
        }
    }
}