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

public class CreateFromBudgetAssignments
{
    public record Command : IRequest<int>
    {
        public int AccountPartitionId { get; set; }
        public DateOnly Date { get; set; }
        public string Recipient { get; set; }
        public ICollection<BudgetAssignment> BudgetAssignments { get; set; }

        public Command(int accountPartitionId, DateOnly date, string recipient, ICollection<BudgetAssignment> budgetAssignments)
        {
            AccountPartitionId = accountPartitionId;
            Date = date;
            Recipient = recipient;
            BudgetAssignments = budgetAssignments;
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
            RuleFor(command => command.AccountPartitionId).GreaterThanOrEqualTo(0);
            RuleFor(command => command.Recipient).NotEmpty();
            RuleForEach(command => command.BudgetAssignments).SetValidator(new CommandBudgetAssignmentValidator());
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
            var partition = await _db.AccountPartitions.Where(ap => ap.Id == request.AccountPartitionId).SingleOrDefaultAsync();
            if (partition == null)
                throw new DomainException("Account partition does not exist.");

            var amount = request.BudgetAssignments.Sum(ba => ba.Amount);
            var transaction = new AccountTransaction(request.Date, amount, partition.Account, partition, request.Recipient);

            var assignments = await Task.WhenAll(request.BudgetAssignments.Select(async item =>
            {
                var category = await _db.BudgetCategories.FindAsync(item.BudgetCategoryId);

                if (category == null)
                    throw new DomainException("Budget category does not exist.");

                return new Entities.BudgetAssignmentTransaction(category, item.Amount, _mapper.Map<BudgetMonth>(item.BudgetMonth), item.Note);
            }));
            transaction.BudgetAssignments.AddRange(assignments);

            await _db.AccountTransactions.AddAsync(transaction);
            await _db.SaveChangesAsync();

            return transaction.Id;
        }
    }
}