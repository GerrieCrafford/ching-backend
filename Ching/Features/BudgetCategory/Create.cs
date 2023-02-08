namespace Ching.Features.BudgetCategory;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using FluentValidation;

public class Create
{
    public record Command(string Name) : IRequest<int>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(command => command.Name).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var budgetCategory = new BudgetCategory(request.Name);
            await _db.BudgetCategories.AddAsync(budgetCategory, cancellationToken);
            await _db.SaveChangesAsync();

            return budgetCategory.Id;
        }
    }
}