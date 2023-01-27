namespace Ching.Features.Overview;

using Microsoft.EntityFrameworkCore;
using MediatR;
using Ching.Data;

public class GetAccountPartitionOverview
{
    public record Query : IRequest<Result>
    {
        public int AccountId { get; init; }
    }

    public record Result
    {
        public List<PartitionDataItem> PartitionData { get; init; }

        public record PartitionDataItem
        {
            public int PartitionId { get; init; }
            public string PartitionName { get; init; }
            public decimal Balance { get; init; }
        }
    }

    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var dataItems = from at in _db.AccountTransactions.Where(x => x.AccountId == request.AccountId)
                            group at by at.AccountPartition
                            into g
                            select new Result.PartitionDataItem
                            {
                                PartitionId = g.Key.Id,
                                PartitionName = g.Key.Name,
                                Balance = (decimal)g.ToList().Sum(at => (float)at.Amount)
                            };

            return new Result { PartitionData = await dataItems.ToListAsync() };
        }
    }
}