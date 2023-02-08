namespace Ching.Features.AccountTransaction;

using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using Ching.DTOs;
using FluentValidation;
using AutoMapper;

public class Edit
{
    public record Command : IRequest
    {
        public int AccountTransactionId { get; set; }
        public DateOnly Date { get; set; }
        public string Recipient { get; set; }
        public string? Note { get; set; }
        public ICollection<BudgetAssignment> BudgetAssignments { get; set; }

        public Command(int accountTransactionId, DateOnly date, string recipient, ICollection<BudgetAssignment> budgetAssignments, string? note)
        {
            AccountTransactionId = accountTransactionId;
            Date = date;
            Recipient = recipient;
            BudgetAssignments = budgetAssignments;
            Note = note;
        }

        public record BudgetAssignment(int BudgetCategoryId, BudgetMonthDTO BudgetMonth, decimal Amount, string? Note = null);
    }

    public class CommandBudgetAssignmentValidator : AbstractValidator<Command.BudgetAssignment>
    {
        public CommandBudgetAssignmentValidator()
        {
            RuleFor(ba => ba.Amount).GreaterThan(0);
            RuleFor(ba => ba.BudgetCategoryId).GreaterThanOrEqualTo(0);
            RuleFor(ba => ba.BudgetMonth.Month).InclusiveBetween(1, 12);
        }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(command => command.AccountTransactionId).GreaterThanOrEqualTo(0);
            RuleFor(command => command.Recipient).NotEmpty();
            RuleForEach(command => command.BudgetAssignments).SetValidator(new CommandBudgetAssignmentValidator());
        }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly ChingContext _db;
        private readonly IMapper _mapper;

        public Handler(ChingContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var transaction = await _db.AccountTransactions.Include(x => x.BudgetAssignments).FirstOrDefaultAsync(x => x.Id == request.AccountTransactionId);

            if (transaction == null)
                throw new DomainException("Transaction does not exist.");

            transaction.Date = request.Date;
            transaction.Recipient = request.Recipient;
            transaction.Note = request.Note;
            transaction.BudgetAssignments.Clear();

            var budgetAssignments = await Task.WhenAll(request.BudgetAssignments.Select(async x =>
            {
                var category = await _db.BudgetCategories.FindAsync(x.BudgetCategoryId);

                if (category == null)
                    throw new DomainException("Budget category does not exist");

                return new BudgetAssignmentTransaction(category, x.Amount, _mapper.Map<BudgetMonth>(x.BudgetMonth));
            }));

            transaction.BudgetAssignments.AddRange(budgetAssignments);

            await _db.SaveChangesAsync();
            return Unit.Value;
        }
    }
}