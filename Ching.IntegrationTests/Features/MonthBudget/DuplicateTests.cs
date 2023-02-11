using Ching.Features.MonthBudget;
using Ching.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ching.IntegrationTests.Features.MonthBudget;

[Collection(nameof(SliceFixture))]
public class DuplicateTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public DuplicateTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_duplicate_months_budgets()
    {
        var mbs = await _fixture.ExecuteDbContextAsync(db => db.MonthBudgets.Where(x => x.BudgetMonth.Year == 2023 && x.BudgetMonth.Month == 1).ToListAsync());

        var command = new Duplicate.Command(2023, 1);
        await _fixture.SendAsync(command);

        var duplicated = await _fixture.ExecuteDbContextAsync(db =>
        {
            return db.MonthBudgets.Where(x => x.BudgetMonth.Month == 2).ToListAsync();
        });

        duplicated.Count.ShouldBe(4);
        duplicated[0].Amount.ShouldBe(mbs[0].Amount);
        duplicated[1].Amount.ShouldBe(mbs[1].Amount);
        duplicated[2].Amount.ShouldBe(mbs[2].Amount);
        duplicated[3].Amount.ShouldBe(mbs[3].Amount);
        duplicated[0].BudgetCategory.Id.ShouldBe(mbs[0].BudgetCategory.Id);
        duplicated[1].BudgetCategory.Id.ShouldBe(mbs[1].BudgetCategory.Id);
        duplicated[2].BudgetCategory.Id.ShouldBe(mbs[2].BudgetCategory.Id);
        duplicated[3].BudgetCategory.Id.ShouldBe(mbs[3].BudgetCategory.Id);
    }

}