namespace Ching.Features.AccountTransaction;

using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;

public class Create
{
    public record Command : IRequest<int>
    {
        public int AccountPartitionId { get; set; }
        public DateOnly Date { get; set; }
        public string? Note { get; set; }
        public ICollection<BudgetAssignment> BudgetAssignments { get; set; }

        public record BudgetAssignment
        {
            public int BudgetCategoryId { get; set; }
            public BudgetMonth BudgetMonth { get; set; }
            public Decimal Amount { get; set; }
            public string? Note { get; set; }
        }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var partition = await _db.AccountPartitions.Where(ap => ap.Id == request.AccountPartitionId).SingleOrDefaultAsync();
            var amount = request.BudgetAssignments.Sum(ba => ba.Amount);
            var transaction = new AccountTransaction(request.Date, amount, partition, request.Note);

            var assignments = request.BudgetAssignments.Select(item => new Entities.BudgetAssignmentTransaction
            {
                Amount = item.Amount,
                BudgetCategoryId = item.BudgetCategoryId,
                BudgetMonth = item.BudgetMonth,
            }).ToList();
            assignments.ForEach(item => transaction.BudgetAssignments.Add(item));

            await _db.AccountTransactions.AddAsync(transaction);
            await _db.SaveChangesAsync();

            return transaction.Id;
        }
    }
}