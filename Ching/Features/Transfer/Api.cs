using AutoMapper;
using MediatR;
using Ching.DTOs;
using FluentValidation;

namespace Ching.Features.Transfer;

public static class TransferEndpoints
{
    public static void MapTransferEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .Produces<int>()
            .WithName("CreateTransfer");

        group.MapPost("/savings-payment", CreateSavingsPayment)
            .Produces<int>()
            .WithName("CreateSavingsPayment");
    }

    public static async Task<IResult> Create(CreateTransfer.Command request, IMediator mediator, IMapper mapper)
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

    public static async Task<IResult> CreateSavingsPayment(CreateSavingsPayment.Command request, IMediator mediator, IMapper mapper)
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
}