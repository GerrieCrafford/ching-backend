using AutoMapper;
using Ching.DTOs;
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

    public static async Task<IResult> Create(CreateBudgetIncreaseRequest request, IMediator mediator, IMapper mapper)
    {
        var id = await mediator.Send(mapper.Map<Create.Command>(request));
        return Results.Ok(id);
    }

    public class MappingProfile : Profile
    {
        MappingProfile()
        {
            CreateMap<CreateBudgetIncreaseRequest, Create.Command>();
            CreateMap<CreateBudgetIncreaseTransferRequest, Create.Command.TransferData>();
        }
    }
}