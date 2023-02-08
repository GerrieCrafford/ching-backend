using AutoMapper;
using MediatR;
using Ching.DTOs;

namespace Ching.Features.Overview;

public static class OverviewEndpoints
{
    public static void MapOverviewEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/account-partition/{accountPartitionId}", AccountPartitionOverview)
            .Produces<List<PartitionBalanceDTO>>()
            .WithName("GetAccountPartitionOverview");

        group.MapGet("/account-transaction/{accountId}", AccountTransactionOverview)
            .Produces<List<TransactionWithBalanceDTO>>()
            .WithName("GetAccountTransactionOverview");

        group.MapGet("/budget/{year}/{month}", BudgetOverview)
            .Produces<List<BudgetOverviewItemDTO>>()
            .WithName("GetBudgetOverview");
    }

    public static async Task<IResult> AccountPartitionOverview(int accountPartitionId, IMediator mediator, IMapper mapper)
    {
        var overview = await mediator.Send(new GetAccountPartitionOverview.Query(accountPartitionId));

        if (overview == null || overview.PartitionData == null) return Results.NotFound();

        return Results.Ok(mapper.Map<List<PartitionBalanceDTO>>(overview.PartitionData));
    }

    public static async Task<IResult> AccountTransactionOverview(int accountId, IMediator mediator, IMapper mapper)
    {
        var overview = await mediator.Send(new GetAccountTransactions.Query(accountId));

        if (overview == null || overview.TransactionData == null) return Results.NotFound();

        return Results.Ok(mapper.Map<List<TransactionWithBalanceDTO>>(overview.TransactionData));
    }

    public static async Task<IResult> BudgetOverview(int year, int month, IMediator mediator, IMapper mapper)
    {
        var overview = await mediator.Send(new GetBudgetOverview.Query(year, month));

        if (overview == null || overview.OverviewItems == null) return Results.NotFound();

        return Results.Ok(mapper.Map<List<BudgetOverviewItemDTO>>(overview.OverviewItems));
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GetAccountPartitionOverview.Result.PartitionDataItem, PartitionBalanceDTO>();
            CreateMap<GetAccountTransactions.Result.Transaction, TransactionDTO>();
            CreateMap<GetAccountTransactions.Result.TransactionWithBalance, TransactionWithBalanceDTO>();
            CreateMap<GetBudgetOverview.Result.BudgetOverviewItem, BudgetOverviewItemDTO>();
        }
    }
}