using FluentValidation;

namespace Ching.DTOs;

public record CreateBudgetCategoryRequest(string Name);

public class CreateBudgetCategoryRequestValidator : AbstractValidator<CreateBudgetCategoryRequest>
{
    public CreateBudgetCategoryRequestValidator()
    {
        RuleFor(req => req.Name).NotEmpty();
    }
}