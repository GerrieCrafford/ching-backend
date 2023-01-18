namespace Ching.Features.AccountPartition;

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
        public int AccountId { get; set; }
        public string Name { get; set; }
        public BudgetMonth? BudgetMonth { get; set; }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var account = await _db.Accounts.Where(acc => acc.Id == request.AccountId).Include(a => a.Partitions).SingleOrDefaultAsync();
            var partition = new AccountPartition(request.Name, request.BudgetMonth);

            account.Partitions.Add(partition);
            await _db.SaveChangesAsync();

            return partition.Id;
        }
    }
}