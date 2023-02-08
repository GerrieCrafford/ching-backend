using MediatR;
using AutoMapper;
using Ching.DTOs;
using FluentValidation;

namespace Ching.Features.Account;

public static class AccountsEndpoints
{
    public static void MapAccountsApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAccounts)
            .Produces<List<AccountDTO>>()
            .WithName("GetAccounts");

        group.MapPost("/", Create)
            .Produces<int>()
            .WithName("CreateAccount");
    }

    public static async Task<IResult> GetAccounts(IMediator mediator, IMapper mapper)
    {
        var data = await mediator.Send(new List.Query());

        if (data == null)
            return Results.Problem();

        var accounts = mapper.Map<List<AccountDTO>>(data.Accounts);
        return Results.Ok(accounts);
    }

    public static async Task<IResult> Create(Create.Command request, IMediator mediator)
    {
        try
        {
            var id = await mediator.Send(request);
            return Results.Ok(id);
        }
        catch (ValidationException exception)
        {
            return Results.BadRequest(new { Errors = exception.Errors });
        }
    }
}