using AutoMapper;
using Ching.DTOs;
using MediatR;

namespace Ching.Features.BudgetCategory;

public static class BudgetCategoriesEndpoints
{
    public static void MapBudgetCategoriesApi(this RouteGroupBuilder group)
    {
        group.MapPost("/", Create)
            .Produces<int>()
            .Produces<List<FluentValidation.Results.ValidationFailure>>(400)
            .WithName("CreateBudgetCategory");
    }

    public static async Task<IResult> Create(CreateBudgetCategoryRequest request, IMediator mediator, IMapper mapper, CreateBudgetCategoryRequestValidator validator)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            return Results.BadRequest(new { Errors = result.Errors });
        }

        var id = await mediator.Send(mapper.Map<Create.Command>(request));
        return Results.Ok(id);
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateBudgetCategoryRequest, Create.Command>();
        }
    }
}