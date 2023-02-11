using Microsoft.EntityFrameworkCore;
using Ching.Features.BudgetCategory;

namespace Ching.IntegrationTests.Features.BudgetCategory;

[Collection(nameof(SliceFixture))]
public class CreateTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public CreateTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_new_budget_category()
    {
        var command = new Create.Command("Category 1");
        var budgetCategoryId = await _fixture.SendAsync(command);

        var created = await _fixture.ExecuteDbContextAsync(db => db.BudgetCategories.Where(cat => cat.Name == command.Name).SingleOrDefaultAsync());

        created.ShouldNotBeNull();
        created.Id.ShouldBe(budgetCategoryId);
        created.Name.ShouldBe(command.Name);

        var command2 = new Create.Command("Category 2", budgetCategoryId);
        var budgetCategoryId2 = await _fixture.SendAsync(command2);

        var created2 = await _fixture.ExecuteDbContextAsync(db => db.BudgetCategories.Include(cat => cat.Parent).SingleOrDefaultAsync(cat => cat.Id == budgetCategoryId2));

        created2.ShouldNotBeNull();
        created2.Parent.ShouldNotBeNull();
        created2.Parent.Id.ShouldBe(budgetCategoryId);
        created2.Parent.Name.ShouldBe(command.Name);
    }
}