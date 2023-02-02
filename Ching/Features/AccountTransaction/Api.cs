using MediatR;
using Ching.DTOs;
using AutoMapper;

namespace Ching.Features.AccountTransaction;

public static class AccountTransactionsEndpoints
{
    public static void MapAccountTransactionsApi(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .Produces<int>()
            .WithName("CreateAccountTransaction");

        group.MapPost("/budget-assignment", CreateFromAssignments)
            .Produces<int>()
            .WithName("CreateAccountTransactionFromAssignments");

        group.MapPatch("/{accountTransactionId}", Edit)
            .Produces(200)
            .WithName("EditAccountTransaction");
    }

    public static async Task<IResult> Create(CreateAccountTransactionRequest request, IMediator mediator, IMapper mapper)
    {
        var id = await mediator.Send(mapper.Map<Create.Command>(request));
        return Results.Ok(id);
    }

    public static async Task<IResult> CreateFromAssignments(CreateAccountTransactionFromAssignmentsRequest request, IMediator mediator, IMapper mapper)
    {
        var id = await mediator.Send(mapper.Map<CreateFromBudgetAssignments.Command>(request));
        return Results.Ok(id);
    }

    public static async Task<IResult> Edit(int accountTransactionId, EditAccountTransactionRequest request, IMediator mediator, IMapper mapper)
    {
        var command = mapper.Map<Edit.Command>(request);
        command.AccountTransactionId = accountTransactionId;

        await mediator.Send(command);

        return Results.Ok();
    }

    public class MappingProfile : Profile
    {
        MappingProfile()
        {
            CreateMap<CreateBudgetAssignmentRequest, CreateFromBudgetAssignments.Command.BudgetAssignment>();
            CreateMap<CreateAccountTransactionFromAssignmentsRequest, CreateFromBudgetAssignments.Command>();

            CreateMap<CreateAccountTransactionRequest, Create.Command>();

            CreateMap<EditAccountTransactionRequest, Edit.Command>();
        }
    }
}