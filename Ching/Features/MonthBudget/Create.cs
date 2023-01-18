namespace Ching.Features.MonthBudget;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using Microsoft.EntityFrameworkCore;

public class Create
{
    public record Command : IRequest<int>
    {
        public int BudgetCategoryId { get; set; }
        public decimal Amount { get; set; }
        public BudgetMonthData BudgetMonth { get; set; }
        public record BudgetMonthData
        {
            public int Month { get; set; }
            public int Year { get; set; }
        }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var cat = await _db.BudgetCategories.Where(x => x.Id == request.BudgetCategoryId).SingleOrDefaultAsync();
            var monthBudget = new MonthBudget(request.Amount, new BudgetMonth(request.BudgetMonth.Year, request.BudgetMonth.Month), cat);

            await _db.MonthBudgets.AddAsync(monthBudget);
            await _db.SaveChangesAsync();

            return monthBudget.Id;
        }
    }
}