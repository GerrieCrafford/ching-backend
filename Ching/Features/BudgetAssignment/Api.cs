using MediatR;
using Ching.DTOs;
using AutoMapper;
using FluentValidation;

namespace Ching.Features.BudgetAssignment;

public static class BudgetAssignmentEndpoints
{
    public static void MapBudgetAssignmentTransactions(this RouteGroupBuilder group)
    {
        group
            .MapGet("/", List)
            .Produces<List<BudgetAssignmentDTO>>()
            .WithName("GetBudgetAssignmentList");
    }

    public static async Task<IResult> List(
        int? budgetCategoryId,
        IMediator mediator,
        IMapper mapper
    )
    {
        try
        {
            var query = new List.Query(budgetCategoryId);
            var response = await mediator.Send(query);

            if (response == null)
                return Results.NotFound();

            var dtos = mapper.Map<List<BudgetAssignmentDTO>>(response.BudgetAssignments);

            return Results.Ok(dtos);
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

    public class MappingProfile : Profile
    {
        public MappingProfile() { }
    }
}
