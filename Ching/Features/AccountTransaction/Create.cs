namespace Ching.Features.AccountTransaction;

using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using FluentValidation;

public class Create
{
    public record Command(int AccountPartitionId, DateOnly Date, decimal Amount, string Recipient, string? Note = null) : IRequest<int>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(command => command.AccountPartitionId).GreaterThanOrEqualTo(0);
            RuleFor(command => command.Recipient).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var accountPartition = await _db.AccountPartitions.FindAsync(request.AccountPartitionId);
            if (accountPartition == null)
                throw new DomainException("Account partition does not exist.");

            var accountTransaction = new AccountTransaction(request.Date, request.Amount, accountPartition.Account, accountPartition, request.Recipient, request.Note);

            await _db.AccountTransactions.AddAsync(accountTransaction);
            await _db.SaveChangesAsync();

            return accountTransaction.Id;
        }
    }
}