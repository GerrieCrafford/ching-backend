namespace Ching.Features.Transfer;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

public class CreateTransfer
{
    public record Command(DateOnly Date, decimal Amount, int SourcePartitionId, int DestinationPartitionId) : IRequest<int>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(command => command.Amount).GreaterThan(0);
            RuleFor(command => command.SourcePartitionId).GreaterThanOrEqualTo(0);
            RuleFor(command => command.DestinationPartitionId).GreaterThanOrEqualTo(0);
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

            var transfer = new Transfer(request.Date, request.Amount, source, dest);

            await _db.Transfers.AddAsync(transfer);
            await _db.SaveChangesAsync();

            return transfer.Id;
        }
    }
}