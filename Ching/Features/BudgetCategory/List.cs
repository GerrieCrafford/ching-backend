using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;

using Ching.Data;

namespace Ching.Features.BudgetCategory;

public class List
{
    public record Query() : IRequest<Result>;

    public record Result
    {
        public List<BudgetCategory> BudgetCategories;

        public Result(List<BudgetCategory> budgetCategories) => BudgetCategories = budgetCategories;

        public record BudgetCategory(int Id, string Name, int? ParentId);
    }

    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly ChingContext _db;

        public Handler(ChingContext db) => _db = db;

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var categories = await _db.BudgetCategories.ToListAsync();

            return new Result(
                categories
                    .Select(cat => new Result.BudgetCategory(cat.Id, cat.Name, cat.ParentId))
                    .ToList()
            );
        }
    }
}
