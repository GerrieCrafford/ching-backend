using Microsoft.EntityFrameworkCore;
using Ching.Features.MonthBudget;

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

        var command = new Create.Command
        {
            BudgetCategoryId = cat1.Id,
            Amount = 13.43m,
            BudgetMonth = new Create.Command.BudgetMonthData
            {
                Month = 5,
                Year = 2023
            }
        };
        var monthBudgetId = await _fixture.SendAsync(command);

        var monthBudget = await _fixture.GetLast<Entities.MonthBudget>();

        monthBudget.Id.ShouldBe(monthBudgetId);
        monthBudget.BudgetCategory.Id.ShouldBe(cat1.Id);
        monthBudget.Amount.ShouldBe(13.43m);
        monthBudget.BudgetMonth.ShouldBeEquivalentTo(new Entities.BudgetMonth(2023, 5));
    }
}