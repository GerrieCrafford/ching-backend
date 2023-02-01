using MediatR;
using Ching.DTOs;

namespace Ching.Features.Account;

public static class AccountsEndpoints
{
    public static void MapAccountsApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAccounts)
            .Produces<List<AccountDTO>>()
            .WithName("GetAccounts");
    }

    public static async Task<IResult> GetAccounts(IMediator mediator)
    {
        var data = await mediator.Send(new List.Query());

        if (data == null)
            return Results.Problem();

        var accounts = data.Accounts.Select(x => new AccountDTO(x.Id, x.Name));
        return Results.Ok(accounts);
    }
}