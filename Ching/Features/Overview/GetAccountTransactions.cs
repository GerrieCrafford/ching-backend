namespace Ching.Features.Overview;

using Microsoft.EntityFrameworkCore;
using MediatR;
using Ching.Data;

public class GetAccountTransactions
{
    public record Query : IRequest<Result>
    {
        public int AccountId { get; init; }
    }

    public record Result
    {
        public List<TransactionWithBalance> TransactionData { get; init; }

        public record TransactionWithBalance
        {
            public Transaction Transaction { get; init; }
            public decimal Balance { get; init; }
        }

        public record Transaction
        {
            public DateOnly Date { get; init; }
            public decimal Amount { get; init; }
            public string? Note { get; init; }
        }
    }

    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var transactions = await _db.AccountTransactions.Where(x => x.AccountId == request.AccountId).OrderBy(x => x.Date).ThenBy(x => x.Id).ToListAsync();

            var balance = 0m;

            var data = new List<Result.TransactionWithBalance>();

            foreach (var transaction in transactions)
            {
                balance += transaction.Amount;
                var transactionWithBalance = new Result.TransactionWithBalance
                {
                    Balance = balance,
                    Transaction = new Result.Transaction
                    {
                        Amount = transaction.Amount,
                        Date = transaction.Date,
                        Note = transaction.Note
                    }
                };
                data.Add(transactionWithBalance);
            }

            return new Result { TransactionData = data };
        }
    }
}