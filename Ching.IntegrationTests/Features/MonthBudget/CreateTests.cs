using Microsoft.EntityFrameworkCore;
using Ching.Features.MonthBudget;
using Ching.DTOs;

namespace Ching.IntegrationTests.Features.MonthBudget;

[Collection(nameof(SliceFixture))]
public class CreateTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public CreateTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_month_budget()
    {
        var cat1 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 1");
        cat1.ShouldNotBeNull();

        var command = new Create.Command
        (
            cat1.Id,
            13.43m,
            new BudgetMonthDTO(2023, 5)
        );
        var monthBudgetId = await _fixture.SendAsync(command);

        var monthBudget = await _fixture.GetLast<Entities.MonthBudget>();
        monthBudget.ShouldNotBeNull();

        monthBudget.Id.ShouldBe(monthBudgetId);
        monthBudget.BudgetCategory.Id.ShouldBe(cat1.Id);
        monthBudget.Amount.ShouldBe(13.43m);
        monthBudget.BudgetMonth.ShouldBeEquivalentTo(new Entities.BudgetMonth(2023, 5));
    }
}