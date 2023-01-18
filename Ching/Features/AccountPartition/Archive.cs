namespace Ching.Features.AccountPartition;

using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;

public class Archive
{
    public record Command : IRequest
    {
        public int AccountPartitionId { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var accountPartition = await _db.AccountPartitions.Where(acc => acc.Id == request.AccountPartitionId).SingleOrDefaultAsync();
            accountPartition.Archived = true;
            await _db.SaveChangesAsync();

            return Unit.Value;
        }
    }
}