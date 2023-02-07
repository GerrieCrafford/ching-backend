using AutoMapper;
using MediatR;
using Ching.DTOs;

namespace Ching.Features.Settlement;

public static class SettlementEndpoints
{
    public static void MapSettlementEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .Produces(200)
            .WithName("CreateSettlement");
    }

    public static async Task<IResult> Create(CreateSettlementRequest request, IMediator mediator, IMapper mapper)
    {
        await mediator.Send(mapper.Map<Create.Command>(request));

        return Results.Ok();
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateSettlementRequest, Create.Command>();
        }
    }
}