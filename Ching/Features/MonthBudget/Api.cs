using AutoMapper;
using Ching.DTOs;
using MediatR;

namespace Ching.Features.MonthBudget;

public static class MonthBudgetsEndpoints
{
    public static void MapMonthBudgetsEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .Produces<int>()
            .WithName("CreateMonthBudget");

        group.MapPost("/duplicate", Duplicate)
            .Produces(200)
            .WithName("DuplicateMonthBudget");
    }

    public static async Task<IResult> Create(CreateMonthBudgetRequest request, IMediator mediator, IMapper mapper)
    {
        var id = await mediator.Send(mapper.Map<Create.Command>(request));
        return Results.Ok(id);
    }

    public static async Task<IResult> Duplicate(int year, int month, IMediator mediator, IMapper mapper)
    {
        await mediator.Send(new Duplicate.Command { Year = year, Month = month });
        return Results.Ok();
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateMonthBudgetRequest, Create.Command>();
        }
    }
}