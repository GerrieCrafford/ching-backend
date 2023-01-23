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
        var (mb1, mb2, mb3) = await _fixture.ExecuteDbContextAsync(async db =>
        {
            var cat1 = await db.BudgetCategories.Where(x => x.Name == "Seed category 1").SingleOrDefaultAsync();
            var cat2 = await db.BudgetCategories.Where(x => x.Name == "Seed category 2").SingleOrDefaultAsync();
            var cat3 = await db.BudgetCategories.Where(x => x.Name == "Seed category 3").SingleOrDefaultAsync();

            var mb1 = new Entities.MonthBudget(113, new BudgetMonth(2023, 1), cat1);
            var mb2 = new Entities.MonthBudget(403, new BudgetMonth(2023, 1), cat2);
            var mb3 = new Entities.MonthBudget(551, new BudgetMonth(2023, 1), cat3);
            await db.MonthBudgets.AddAsync(mb1);
            await db.MonthBudgets.AddAsync(mb2);
            await db.MonthBudgets.AddAsync(mb3);
            await db.SaveChangesAsync();

            return (mb1, mb2, mb3);
        });

        var command = new Duplicate.Command
        {
            Month = 1,
            Year = 2023
        };
        await _fixture.SendAsync(command);

        var duplicated = await _fixture.ExecuteDbContextAsync(db =>
        {
            return db.MonthBudgets.Where(x => x.BudgetMonth.Month == 2).ToListAsync();
        });

        duplicated.Count.ShouldBe(3);
        duplicated[0].Amount.ShouldBe(mb1.Amount);
        duplicated[1].Amount.ShouldBe(mb2.Amount);
        duplicated[2].Amount.ShouldBe(mb3.Amount);
        duplicated[0].BudgetCategory.Id.ShouldBe(mb1.BudgetCategory.Id);
        duplicated[1].BudgetCategory.Id.ShouldBe(mb2.BudgetCategory.Id);
        duplicated[2].BudgetCategory.Id.ShouldBe(mb3.BudgetCategory.Id);
    }

}