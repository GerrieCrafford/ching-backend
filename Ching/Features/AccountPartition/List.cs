namespace Ching.Features.AccountPartition;

using Microsoft.EntityFrameworkCore;
using MediatR;
using Ching.Data;
using AutoMapper;
using Ching.DTOs;

public class List
{
    public record Query : IRequest<Result>
    {
        public int AccountId { get; init; }
    }

    public record Result
    {
        public List<AccountPartition> AccountPartitions { get; init; }

        public record AccountPartition
        {
            public int Id { get; init; }
            public string Name { get; init; }
            public bool Archived { get; init; }
            public BudgetMonthDTO? BudgetMonth { get; init; }
        }
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
            return new Result
            {
                AccountPartitions = partitions.Select(x => new Result.AccountPartition
                {
                    Id = x.Id,
                    Name = x.Name,
                    Archived = x.Archived,
                    BudgetMonth = x.BudgetMonth != null ? new BudgetMonthDTO(x.BudgetMonth.Year, x.BudgetMonth.Month) : null
                }).ToList()
            };
        }
    }
}