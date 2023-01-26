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
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var accountPartition = await _db.AccountPartitions.FindAsync(request.AccountPartitionId);

            var accountTransaction = new AccountTransaction(request.Date, request.Amount, accountPartition.Account, accountPartition, request.Note);

            await _db.AccountTransactions.AddAsync(accountTransaction);
            await _db.SaveChangesAsync();

            return accountTransaction.Id;
        }
    }
}