namespace Ching.Features.BudgetCategory;

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
            var budgetCategory = new BudgetCategory { Name = request.Name };
            await _db.BudgetCategories.AddAsync(budgetCategory, cancellationToken);
            await _db.SaveChangesAsync();

            return budgetCategory.Id;
        }
    }
}