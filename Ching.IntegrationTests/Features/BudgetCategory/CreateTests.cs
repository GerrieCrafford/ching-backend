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
    }
}