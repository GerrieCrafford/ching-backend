namespace Ching.Features.AccountPartition;

using Microsoft.EntityFrameworkCore;
using MediatR;
using Ching.Data;
using AutoMapper;
using Ching.DTOs;
using FluentValidation;

public class List
{
    public record Query : IRequest<Result>
    {
        public int AccountId { get; init; }
    }

    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(query => query.AccountId).GreaterThanOrEqualTo(0);
        }
    }

    public record Result
    {
        public List<AccountPartition> AccountPartitions { get; init; }

        public Result(List<AccountPartition> accountPartitions) => AccountPartitions = accountPartitions;

        public record AccountPartition(int Id, string Name, bool Archived, BudgetMonthDTO? BudgetMonth);
    }

    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateMap<Result.AccountPartition, AccountPartitionDTO>();
    }

    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var partitions = await _db.AccountPartitions.Where(x => x.AccountId == request.AccountId).ToListAsync();
            return new Result(partitions.Select(x => new Result.AccountPartition
                (
                    x.Id,
                    x.Name,
                    x.Archived,
                    x.BudgetMonth != null ? new BudgetMonthDTO(x.BudgetMonth.Year, x.BudgetMonth.Month) : null
                )).ToList()
            );
        }
    }
}