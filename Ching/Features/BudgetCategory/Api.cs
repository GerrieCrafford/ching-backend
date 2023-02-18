using AutoMapper;
using Ching.DTOs;
using FluentValidation;
using MediatR;

namespace Ching.Features.BudgetCategory;

public static class BudgetCategoriesEndpoints
{
    public static void MapBudgetCategoriesApi(this RouteGroupBuilder group)
    {
        group
            .MapPost("/", CreateHandler)
            .Produces<int>()
            .Produces<List<FluentValidation.Results.ValidationFailure>>(400)
            .WithName("CreateBudgetCategory");

        group
            .MapGet("/", GetList)
            .Produces<List<BudgetCategoryDTO>>()
            .WithName("GetBudgetCategoryList");
    }

    public static async Task<IResult> GetList(IMediator mediator, IMapper mapper)
    {
        try
        {
            var res = await mediator.Send(new List.Query());
            return Results.Ok(mapper.Map<List<BudgetCategoryDTO>>(res.BudgetCategories));
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

    public static async Task<IResult> CreateHandler(
        Create.Command request,
        IMediator mediator,
        IMapper mapper,
        Create.CommandValidator validator
    )
    {
        try
        {
            var result = await validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                return Results.BadRequest(new { Errors = result.Errors });
            }

            var id = await mediator.Send(mapper.Map<Create.Command>(request));
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

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<List.Result.BudgetCategory, BudgetCategoryDTO>();
        }
    }
}
