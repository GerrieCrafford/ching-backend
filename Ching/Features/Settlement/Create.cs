namespace Ching.Features.Settlement;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

public class Create
{
    public record Command(DateOnly Date, List<int> AccountTransactionIds, int SourcePartitionId) : IRequest;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(command => command.SourcePartitionId).GreaterThanOrEqualTo(0);
            RuleFor(command => command.AccountTransactionIds).NotEmpty();
            RuleForEach(command => command.AccountTransactionIds).GreaterThanOrEqualTo(0);
        }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var source = await _db.AccountPartitions.Where(x => x.Id == request.SourcePartitionId).SingleOrDefaultAsync();
            var accountTransactions = _db.AccountTransactions.Where(x => request.AccountTransactionIds.Contains(x.Id));

            var query = from at in accountTransactions
                        group at by at.AccountPartition.Account
                        into g
                        select new { Account = g.Key, Transactions = g.ToList() };

            await query.ForEachAsync(async x =>
            {
                var total = x.Transactions.Sum(x => x.Amount);
                var transfer = new Transfer(request.Date, total, source, x.Account.RemainingPartition);
                var settlement = new Settlement(request.Date, transfer, x.Transactions);

                await _db.Settlements.AddAsync(settlement);

                await _db.SaveChangesAsync();
            });

            return Unit.Value;
        }
    }
}