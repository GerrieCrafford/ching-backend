using AutoMapper;
using Ching.DTOs;
using FluentValidation;
using MediatR;

namespace Ching.Features.BudgetIncrease;

public static class BudgetIncreases
{
    public static void MapBudgetIncreasesApi(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .Produces<int>()
            .WithName("CreateBudgetIncrease");
    }

    public static async Task<IResult> Create(Create.Command request, IMediator mediator, IMapper mapper)
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