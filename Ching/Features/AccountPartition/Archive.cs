namespace Ching.Features.AccountPartition;

using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using FluentValidation;

public class Archive
{
    public record Command(int AccountPartitionId) : IRequest;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(command => command.AccountPartitionId).GreaterThanOrEqualTo(0);
        }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var accountPartition = await _db.AccountPartitions.Where(acc => acc.Id == request.AccountPartitionId).SingleOrDefaultAsync();
            if (accountPartition == null)
                throw new DomainException("Account partition does not exist.");

            accountPartition.Archived = true;
            await _db.SaveChangesAsync();

            return Unit.Value;
        }
    }
}