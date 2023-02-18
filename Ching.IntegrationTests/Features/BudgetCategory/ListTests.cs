using Ching.Features.BudgetCategory;

namespace Ching.IntegrationTests.Features.BudgetCategory;

[Collection(nameof(SliceFixture))]
public class ListTests : BaseTest
{
    private readonly SliceFixture _fixture;

    public ListTests(SliceFixture fixture)
        : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_return_budget_categories()
    {
        var query = new List.Query { };

        var result = await _fixture.SendAsync(query);

        result.ShouldNotBeNull();

        result.BudgetCategories
            .Select(cat => cat.Name)
            .ShouldBe(
                new[] { "Seed category 1", "Seed category 2", "Seed category 3", "Seed category 4" }
            );
        var parent = result.BudgetCategories.ElementAt(2);
        var child = result.BudgetCategories.ElementAt(3);
        child.ParentId.ShouldBe(parent.Id);
    }
}
