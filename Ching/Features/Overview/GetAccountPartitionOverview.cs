namespace Ching.Features.Overview;

using Microsoft.EntityFrameworkCore;
using MediatR;
using Ching.Data;
using FluentValidation;

public class GetAccountPartitionOverview
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
        public List<PartitionDataItem> PartitionData { get; init; }

        public Result(List<PartitionDataItem> dataItems) => PartitionData = dataItems;

        public record PartitionDataItem(int PartitionId, string PartitionName, decimal Balance);
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
                            (
                                g.Key.Id,
                                g.Key.Name,
                                (decimal)g.ToList().Sum(at => (float)at.Amount)
                            );

            return new Result(await dataItems.ToListAsync());
        }
    }
}