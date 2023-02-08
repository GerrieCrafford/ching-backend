namespace Ching.Features.Transfer;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using Microsoft.EntityFrameworkCore;
using Ching.DTOs;
using FluentValidation;
using AutoMapper;

public class CreateSavingsPayment
{
    public record Command : IRequest<int>
    {
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public int SourcePartitionId { get; set; }
        public int DestinationPartitionId { get; set; }
        public BudgetAssignmentData BudgetAssignment { get; set; }

        public Command(DateOnly date, decimal amount, int sourcePartitionId, int destinationPartitionId, BudgetAssignmentData budgetAssignment)
        {
            Date = date;
            Amount = amount;
            SourcePartitionId = sourcePartitionId;
            DestinationPartitionId = destinationPartitionId;
            BudgetAssignment = budgetAssignment;
        }

        public record BudgetAssignmentData(int BudgetCategoryId, BudgetMonthDTO BudgetMonth, decimal Amount, string? Note = null);
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(command => command.Amount).GreaterThan(0);
            RuleFor(command => command.SourcePartitionId).GreaterThanOrEqualTo(0);
            RuleFor(command => command.DestinationPartitionId).GreaterThanOrEqualTo(0);

            RuleFor(command => command.BudgetAssignment.Amount).GreaterThan(0);
            RuleFor(command => command.BudgetAssignment.BudgetCategoryId).GreaterThanOrEqualTo(0);
            RuleFor(command => command.BudgetAssignment.BudgetMonth.Month).InclusiveBetween(1, 12);
        }
    }

    public class Handler : IRequestHandler<Command, int>
    {
        private readonly ChingContext _db;

        public Handler(ChingContext db, IMapper mapper) => _db = db;

        public async Task<int> Handle(Command request, CancellationToken cancellationToken)
        {
            var source = await _db.AccountPartitions.Where(x => x.Id == request.SourcePartitionId).SingleOrDefaultAsync();
            var dest = await _db.AccountPartitions.Where(x => x.Id == request.DestinationPartitionId).SingleOrDefaultAsync();
            var category = await _db.BudgetCategories.Where(x => x.Id == request.BudgetAssignment.BudgetCategoryId).SingleOrDefaultAsync();

            if (source == null)
                throw new DomainException("Source partition does not exist.");

            if (dest == null)
                throw new DomainException("Destination partition does not exist.");

            if (category == null)
                throw new DomainException("Budget category does not exist.");

            var budgetMonth = new BudgetMonth(request.BudgetAssignment.BudgetMonth.Year, request.BudgetAssignment.BudgetMonth.Month);
            var budgetAssignment = new BudgetAssignmentTransfer(category, request.BudgetAssignment.Amount, budgetMonth);

            var transfer = new Transfer(request.Date, request.Amount, source, dest, budgetAssignment);

            await _db.Transfers.AddAsync(transfer);
            await _db.SaveChangesAsync();

            return transfer.Id;
        }
    }
}