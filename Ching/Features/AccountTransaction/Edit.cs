namespace Ching.Features.AccountTransaction;

using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;

public class Edit
{
    public record Command : IRequest
    {
        public int AccountTransactionId { get; set; }
        public DateOnly Date { get; init; }
        public string? Note { get; init; }
        public string Recipient { get; init; }
        public ICollection<BudgetAssignment> BudgetAssignments { get; init; }

        public record BudgetAssignment
        {
            public int BudgetCategoryId { get; init; }
            public BudgetMonth BudgetMonth { get; init; }
            public Decimal Amount { get; init; }
            public string? Note { get; init; }
        }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var transaction = await _db.AccountTransactions.Include(x => x.BudgetAssignments).FirstOrDefaultAsync(x => x.Id == request.AccountTransactionId);

            transaction.Date = request.Date;
            transaction.Recipient = request.Recipient;
            transaction.Note = request.Note;
            transaction.BudgetAssignments.Clear();

            var budgetAssignments = await Task.WhenAll(request.BudgetAssignments.Select(async x =>
            {
                var category = await _db.BudgetCategories.FindAsync(x.BudgetCategoryId);

                return new BudgetAssignmentTransaction(category, x.Amount, x.BudgetMonth);
            }));

            transaction.BudgetAssignments.AddRange(budgetAssignments);

            await _db.SaveChangesAsync();
            return Unit.Value;
        }
    }
}