namespace Ching.Features.Account;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;

public class Create
{
    public record Command : IRequest<int>
    {
        public string Name { get; set; }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var account = new Account(request.Name);
            await _db.Accounts.AddAsync(account, cancellationToken);
            await _db.SaveChangesAsync();

            return account.Id;
        }
    }
}