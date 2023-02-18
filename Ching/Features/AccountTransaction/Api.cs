using MediatR;
using Ching.DTOs;
using AutoMapper;
using FluentValidation;

namespace Ching.Features.AccountTransaction;

public static class AccountTransactionsEndpoints
{
    public static void MapAccountTransactionsApi(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create).Produces<int>().WithName("CreateAccountTransaction");

        group
            .MapPost("/budget-assignment", CreateFromAssignments)
            .Produces<int>()
            .WithName("CreateAccountTransactionFromAssignments");

        group
            .MapPatch("/{accountTransactionId}", Edit)
            .Produces(200)
            .WithName("EditAccountTransaction");
    }

    public static async Task<IResult> Create(
        Create.Command request,
        IMediator mediator,
        IMapper mapper
    )
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

    public static async Task<IResult> CreateFromAssignments(
        CreateFromBudgetAssignments.Command request,
        IMediator mediator,
        IMapper mapper
    )
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

    public static async Task<IResult> Edit(
        int accountTransactionId,
        EditAccountTransactionRequest request,
        IMediator mediator,
        IMapper mapper
    )
    {
        var command = mapper.Map<Edit.Command>(
            request,
            opt => opt.Items["AccountTransactionId"] = accountTransactionId
        );

        await mediator.Send(command);

        return Results.Ok();
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateBudgetAssignmentRequest, Edit.Command.BudgetAssignment>();
            CreateMap<EditAccountTransactionRequest, Edit.Command>()
                .ForCtorParam(
                    "accountTransactionId",
                    opts => opts.MapFrom((_, ctx) => ctx.Items["AccountTransactionId"])
                );
        }
    }
}
