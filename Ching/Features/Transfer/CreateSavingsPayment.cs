namespace Ching.Features.Transfer;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using Microsoft.EntityFrameworkCore;

public class CreateSavingsPayment
{
    public record Command : IRequest<int>
    {
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public int SourcePartitionId { get; set; }
        public int DestinationPartitionId { get; set; }
        public BudgetAssignmentData? BudgetAssignment { get; set; }

        public record BudgetAssignmentData
        {
            public int BudgetCategoryId { get; set; }
            public BudgetMonthData BudgetMonth { get; set; }
            public Decimal Amount { get; set; }
            public string? Note { get; set; }
        }

        public record BudgetMonthData
        {
            public int Year { get; set; }
            public int Month { get; set; }
        }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var source = await _db.AccountPartitions.Where(x => x.Id == request.SourcePartitionId).SingleOrDefaultAsync();
            var dest = await _db.AccountPartitions.Where(x => x.Id == request.DestinationPartitionId).SingleOrDefaultAsync();
            var category = await _db.BudgetCategories.Where(x => x.Id == request.BudgetAssignment.BudgetCategoryId).SingleOrDefaultAsync();

            var budgetMonth = new BudgetMonth(request.BudgetAssignment.BudgetMonth.Year, request.BudgetAssignment.BudgetMonth.Month);
            var budgetAssignment = new BudgetAssignmentTransfer(category, request.BudgetAssignment.Amount, budgetMonth);

            var transfer = new Transfer(request.Date, request.Amount, source, dest, budgetAssignment);

            await _db.Transfers.AddAsync(transfer);
            await _db.SaveChangesAsync();

            return transfer.Id;
        }
    }
}