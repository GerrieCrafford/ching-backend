namespace Ching.Features.BudgetIncrease;

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
    public record Command : IRequest
    {
        public int BudgetCategoryId { get; set; }
        public BudgetMonthDTO BudgetMonth { get; set; }
        public TransferData Transfer { get; set; }

        public Command(int budgetCategoryId, BudgetMonthDTO budgetMonth, TransferData transferData)
        {
            BudgetCategoryId = budgetCategoryId;
            BudgetMonth = budgetMonth;
            Transfer = transferData;
        }

        public record TransferData(DateOnly Date, decimal Amount, int SourcePartitionId, int DestinationPartitionId);
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(command => command.BudgetCategoryId).GreaterThanOrEqualTo(0);
            RuleFor(command => command.BudgetMonth.Month).InclusiveBetween(1, 12);
            RuleFor(command => command.Transfer.Amount).GreaterThan(0);
            RuleFor(command => command.Transfer.SourcePartitionId).GreaterThanOrEqualTo(0);
            RuleFor(command => command.Transfer.DestinationPartitionId).GreaterThanOrEqualTo(0);
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
            var source = await _db.AccountPartitions.Where(x => x.Id == request.Transfer.SourcePartitionId).SingleOrDefaultAsync();
            var dest = await _db.AccountPartitions.Where(x => x.Id == request.Transfer.DestinationPartitionId).SingleOrDefaultAsync();
            var cat = await _db.BudgetCategories.Where(x => x.Id == request.BudgetCategoryId).SingleOrDefaultAsync();

            if (source == null)
                throw new DomainException("Source partition does not exist.");

            if (dest == null)
                throw new DomainException("Destination partition does not exist.");

            if (cat == null)
                throw new DomainException("Budget category does not exist.");

            var transfer = new Transfer(request.Transfer.Date, request.Transfer.Amount, source, dest);
            var increase = new BudgetIncrease(transfer, _mapper.Map<BudgetMonth>(request.BudgetMonth), cat);

            await _db.Transfers.AddAsync(transfer);
            await _db.BudgetIncreases.AddAsync(increase);

            await _db.SaveChangesAsync();
            return Unit.Value;
        }
    }
}