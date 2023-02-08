using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;

using Ching.Data;
using Ching.DTOs;

namespace Ching.Features.Account;

public class List
{
    public record Query() : IRequest<Result>;

    public record Result
    {
        public List<Account> Accounts { get; init; }

        public Result(List<Account> accounts) => Accounts = accounts;

        public record Account(int Id, string Name);
    }

    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly ChingContext _db;
        public Handler(ChingContext db) => _db = db;

        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var accounts = await _db.Accounts.Select(x => new Result.Account
            (
                x.Id,
                x.Name
            )).ToListAsync();

            return new Result(accounts);
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Result.Account, AccountDTO>();
        }
    }
}