namespace Ching.Features.BudgetCategory;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using FluentValidation;

public class Create
{
    public record Command(string Name, int? ParentId = null) : IRequest<int>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(command => command.Name).NotEmpty();
            RuleFor(command => command.ParentId).GreaterThanOrEqualTo(0).Unless(command => command.ParentId == null);
        }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            BudgetCategory? parent = null;
            if (request.ParentId != null)
            {
                parent = await _db.BudgetCategories.FindAsync(request.ParentId);
                if (parent == null)
                    throw new DomainException("Parent budget category does not exist.");
            }

            var budgetCategory = new BudgetCategory(request.Name, parent);
            await _db.BudgetCategories.AddAsync(budgetCategory, cancellationToken);
            await _db.SaveChangesAsync();

            return budgetCategory.Id;
        }
    }
}