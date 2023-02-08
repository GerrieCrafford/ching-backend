using AutoMapper;
using MediatR;
using Ching.DTOs;
using FluentValidation;

namespace Ching.Features.Settlement;

public static class SettlementEndpoints
{
    public static void MapSettlementEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .Produces(200)
            .WithName("CreateSettlement");
    }

    public static async Task<IResult> Create(Create.Command request, IMediator mediator, IMapper mapper)
    {
        try
        {
            await mediator.Send(request);
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