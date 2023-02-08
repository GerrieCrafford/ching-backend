using AutoMapper;
using Ching.DTOs;
using FluentValidation;
using MediatR;

namespace Ching.Features.MonthBudget;

public static class MonthBudgetsEndpoints
{
    public static void MapMonthBudgetsEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .Produces<int>()
            .WithName("CreateMonthBudget");

        group.MapPost("/{year}/{month}/duplicate", Duplicate)
            .Produces(200)
            .WithName("DuplicateMonthBudget");
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

    public static async Task<IResult> Duplicate(int year, int month, IMediator mediator, IMapper mapper)
    {
        try
        {
            await mediator.Send(new Duplicate.Command(year, month));
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