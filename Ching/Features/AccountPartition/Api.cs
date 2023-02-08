using MediatR;
using Ching.DTOs;
using AutoMapper;
using FluentValidation;

namespace Ching.Features.AccountPartition;

public static class AccountPartitionsEndpoints
{
    public static void MapAccountPartitionsApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAccountPartitions)
            .Produces<List<AccountPartitionDTO>>()
            .WithName("GetAccountPartitions");

        group.MapPost("/", CreateAccountPartition)
            .Produces<int>()
            .WithName("CreateAccountPartition");

        group.MapPost("/{accountPartitionId}/archive", ArchivePartition)
            .Produces(200)
            .WithName("ArchiveAccountPartition");
    }

    public static async Task<IResult> GetAccountPartitions(int accountId, IMediator mediator, IMapper mapper)
    {
        try
        {
            var data = await mediator.Send(new List.Query { AccountId = accountId });

            if (data == null)
                return Results.Problem();

            var accountPartitionDTOs = mapper.Map<List<AccountPartitionDTO>>(data.AccountPartitions);
            return Results.Ok(accountPartitionDTOs);
        }
        catch (ValidationException exception)
        {
            return Results.BadRequest(new { Errors = exception.Errors });
        }
    }

    public static async Task<IResult> CreateAccountPartition(Create.Command request, IMediator mediator, IMapper mapper)
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
        catch (DomainException exception)
        {
            return Results.BadRequest(exception.ToResult());
        }
    }

    public static async Task<IResult> ArchivePartition(int accountPartitionId, IMediator mediator)
    {
        try
        {
            await mediator.Send(new Archive.Command(accountPartitionId));
            return Results.Ok();
        }
        catch (ValidationException exception)
        {
            return Results.BadRequest(new { Errors = exception.Errors });
        }
        catch (DomainException exception)
        {
            return Results.BadRequest(exception.ToResult());
        }
    }
}