namespace Ching.Features.Account;

using Microsoft.EntityFrameworkCore;
using MediatR;
using Ching.Data;

public class List
{
    public record Query : IRequest<Result>
    { }

    public record Result
    {
        public List<Account> Accounts { get; init; }

        public record Account
        {
            public int Id { get; init; }
            public string Name { get; init; }
        }
    }

    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var accounts = await _db.Accounts.Select(x => new Result.Account
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return new Result { Accounts = accounts };
        }
    }
}