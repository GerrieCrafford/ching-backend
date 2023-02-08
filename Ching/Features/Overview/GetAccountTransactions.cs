namespace Ching.Features.Overview;

using Microsoft.EntityFrameworkCore;
using MediatR;
using Ching.Data;
using FluentValidation;

public class GetAccountTransactions
{
    public record Query(int AccountId) : IRequest<Result>;

    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(query => query.AccountId).GreaterThanOrEqualTo(0);
        }
    }

    public record Result
    {
        public List<TransactionWithBalance> TransactionData { get; init; }

        public Result(List<TransactionWithBalance> items) => TransactionData = items;

        public record TransactionWithBalance(Transaction Transaction, decimal Balance);
        public record Transaction(DateOnly Date, decimal Amount, string? Note = null);
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
                (
                    new Result.Transaction
                    (
                        transaction.Date,
                        transaction.Amount,
                        transaction.Note
                    ),
                    balance
                );
                data.Add(transactionWithBalance);
            }

            return new Result(data);
        }
    }
}