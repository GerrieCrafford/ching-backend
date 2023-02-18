namespace Ching.Features.BudgetAssignment;

using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;
using FluentValidation;
using Ching.Data;
using Ching.DTOs;
using Ching.Entities;

public class List
{
    public record Query(int? BudgetCategoryId = null) : IRequest<Result>;

    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(query => query.BudgetCategoryId)
                .GreaterThanOrEqualTo(0)
                .Unless(query => query.BudgetCategoryId == null);
        }
    }

    public record Result(List<BudgetAssignmentDTO> BudgetAssignments);

    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly ChingContext _db;
        private readonly IMapper _mapper;

        public Handler(ChingContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            IQueryable<BudgetAssignmentTransaction> transactionAssignmentsQuery =
                _db.BudgetAssignmentsTransactions.OrderByDescending(x => x.AccountTransaction.Date);

            if (request.BudgetCategoryId != null)
                transactionAssignmentsQuery = transactionAssignmentsQuery.Where(
                    a => a.BudgetCategoryId == request.BudgetCategoryId
                );

            IQueryable<BudgetAssignmentTransfer> transferAssignmentsQuery =
                _db.BudgetAssignmentsTransfers;

            if (request.BudgetCategoryId != null)
                transferAssignmentsQuery = transferAssignmentsQuery.Where(
                    a => a.BudgetCategoryId == request.BudgetCategoryId
                );

            var transactionAssignments = await transactionAssignmentsQuery.ToListAsync();
            var transferAssignments = await transferAssignmentsQuery.ToListAsync();

            var assignments1 = _mapper.Map<List<BudgetAssignmentDTO>>(transactionAssignments);
            var assignments2 = _mapper.Map<List<BudgetAssignmentDTO>>(transferAssignments);

            var assignments = Enumerable
                .Concat(assignments1, assignments2)
                .OrderByDescending(x => x.Date)
                .ToList();
            return new Result(assignments);
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<BudgetAssignmentTransaction, BudgetAssignmentDTO>()
                    .ConvertUsing(
                        x =>
                            new BudgetAssignmentDTO(
                                x.Id,
                                x.AccountTransaction.Date,
                                BudgetAssignmentType.Transaction,
                                x.BudgetCategoryId,
                                x.BudgetCategory.Name,
                                new BudgetMonthDTO(x.BudgetMonth.Year, x.BudgetMonth.Month),
                                x.Amount,
                                x.AccountTransaction.Recipient,
                                x.Note
                            )
                    );

                CreateMap<BudgetAssignmentTransfer, BudgetAssignmentDTO>()
                    .ConvertUsing(
                        x =>
                            new BudgetAssignmentDTO(
                                x.Id,
                                x.Transfer.Date,
                                BudgetAssignmentType.Transfer,
                                x.BudgetCategoryId,
                                x.BudgetCategory.Name,
                                new BudgetMonthDTO(x.BudgetMonth.Year, x.BudgetMonth.Month),
                                x.Amount,
                                "Internal",
                                x.Note
                            )
                    );
            }
        }
    }
}
