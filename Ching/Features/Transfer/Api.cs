using AutoMapper;
using MediatR;
using Ching.DTOs;

namespace Ching.Features.Transfer;

public static class TransferEndpoints
{
    public static void MapTransferEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .Produces<int>()
            .WithName("CreateTransfer");

        group.MapPost("/savings-payment", CreateSavingsPayment)
            .Produces<int>()
            .WithName("CreateSavingsPayment");
    }

    public static async Task<IResult> Create(CreateTransferRequest request, IMediator mediator, IMapper mapper)
    {
        var id = await mediator.Send(mapper.Map<CreateTransfer.Command>(request));
        return Results.Ok(id);
    }

    public static async Task<IResult> CreateSavingsPayment(CreateSavingsPaymentRequest request, IMediator mediator, IMapper mapper)
    {
        var id = await mediator.Send(mapper.Map<CreateSavingsPayment.Command>(request));
        return Results.Ok(id);
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateTransferRequest, CreateTransfer.Command>();

            CreateMap<CreateBudgetAssignmentRequest, CreateSavingsPayment.Command.BudgetAssignmentData>();
            CreateMap<CreateMonthBudgetRequest, CreateSavingsPayment.Command.BudgetMonthData>();
            CreateMap<CreateSavingsPaymentRequest, CreateSavingsPayment.Command>();
        }
    }
}