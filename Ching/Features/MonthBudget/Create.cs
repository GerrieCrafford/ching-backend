namespace Ching.Features.MonthBudget;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using Microsoft.EntityFrameworkCore;
using Ching.DTOs;
using FluentValidation;
using AutoMapper;

public class Create
{
    public record Command(int BudgetCategoryId, decimal Amount, BudgetMonthDTO BudgetMonth) : IRequest<int>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(command => command.BudgetCategoryId).GreaterThanOrEqualTo(0);
            RuleFor(command => command.Amount).GreaterThan(0);
            RuleFor(command => command.BudgetMonth.Month).InclusiveBetween(1, 12);
        }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly ChingContext _db;
        private readonly IMapper _mapper;

        public Handler(ChingContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var cat = await _db.BudgetCategories.Where(x => x.Id == request.BudgetCategoryId).SingleOrDefaultAsync();

            if (cat == null)
                throw new DomainException("Budget category does not exist.");

            var monthBudget = new MonthBudget(request.Amount, _mapper.Map<BudgetMonth>(request.BudgetMonth), cat);

            await _db.MonthBudgets.AddAsync(monthBudget);
            await _db.SaveChangesAsync();

            return monthBudget.Id;
        }
    }
}