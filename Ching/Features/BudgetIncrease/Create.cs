namespace Ching.Features.BudgetIncrease;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using Microsoft.EntityFrameworkCore;

public class Create
{
    public record Command : IRequest
    {
        public int BudgetCategoryId { get; set; }
        public BudgetMonthData BudgetMonth { get; set; }
        public TransferData Transfer { get; set; }

        public record BudgetMonthData
        {
            public int Month { get; set; }
            public int Year { get; set; }
        }
        public record TransferData
        {
            public DateOnly Date { get; set; }
            public decimal Amount { get; set; }
            public int SourcePartitionId { get; set; }
            public int DestinationPartitionId { get; set; }
        }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var source = await _db.AccountPartitions.Where(x => x.Id == request.Transfer.SourcePartitionId).SingleOrDefaultAsync();
            var dest = await _db.AccountPartitions.Where(x => x.Id == request.Transfer.DestinationPartitionId).SingleOrDefaultAsync();
            var cat = await _db.BudgetCategories.Where(x => x.Id == request.BudgetCategoryId).SingleOrDefaultAsync();

            var transfer = new Transfer(request.Transfer.Date, request.Transfer.Amount, source, dest);
            var increase = new BudgetIncrease(transfer, new BudgetMonth(request.BudgetMonth.Year, request.BudgetMonth.Month), cat);

            await _db.Transfers.AddAsync(transfer);
            await _db.BudgetIncreases.AddAsync(increase);

            await _db.SaveChangesAsync();
            return Unit.Value;
        }
    }
}