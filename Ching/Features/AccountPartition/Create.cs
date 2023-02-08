namespace Ching.Features.AccountPartition;

using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ching.Data;
using Ching.Entities;
using Ching.DTOs;

public class Create
{
    public record Command(int AccountId, string Name, BudgetMonthDTO? BudgetMonth = null) : IRequest<int>;

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
            var account = await _db.Accounts.Where(acc => acc.Id == request.AccountId).Include(a => a.Partitions).SingleOrDefaultAsync();
            if (account == null)
                throw new DomainException("Account does not exist.");

            var partition = new AccountPartition(request.Name, _mapper.Map<BudgetMonth>(request.BudgetMonth));

            account.Partitions.Add(partition);
            await _db.SaveChangesAsync();

            return partition.Id;
        }
    }
}