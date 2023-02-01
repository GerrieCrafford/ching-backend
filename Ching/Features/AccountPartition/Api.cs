using MediatR;
using Ching.DTOs;
using AutoMapper;

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
        var data = await mediator.Send(new List.Query { AccountId = accountId });

        if (data == null)
            return Results.Problem();

        var accountPartitionDTOs = mapper.Map<List<AccountPartitionDTO>>(data.AccountPartitions);
        return Results.Ok(accountPartitionDTOs);
    }

    public static async Task<IResult> CreateAccountPartition(CreateAccountPartitionRequest request, IMediator mediator, IMapper mapper)
    {
        var id = await mediator.Send(mapper.Map<Create.Command>(request));
        return Results.Ok(id);
    }

    public static async Task<IResult> ArchivePartition(int accountPartitionId, IMediator mediator)
    {
        await mediator.Send(new Archive.Command { AccountPartitionId = accountPartitionId });

        return Results.Ok();
    }

    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateMap<CreateAccountPartitionRequest, Create.Command>();
    }
}