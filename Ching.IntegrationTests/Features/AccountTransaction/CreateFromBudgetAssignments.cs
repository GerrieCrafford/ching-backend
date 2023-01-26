using Microsoft.EntityFrameworkCore;
using Ching.Features.AccountTransaction;

namespace Ching.IntegrationTests.Features.AccountTransaction;

[Collection(nameof(SliceFixture))]
public class CreateFromBudgetAssignmentsTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public CreateFromBudgetAssignmentsTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_new_account_transaction_with_budget_assignments()
    {
        var account = await _fixture.FindAsync<Entities.Account>(a => a.Name == "ACC1");
        var partition = await _fixture.FindAsync<Entities.AccountPartition>(p => p.Name == "ACC1P1");
        var cat1 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 1");
        var cat2 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 2");

        var command = new CreateFromBudgetAssignments.Command
        {
            AccountPartitionId = partition.Id,
            Date = new DateOnly(2023, 1, 5),
            BudgetAssignments = new List<CreateFromBudgetAssignments.Command.BudgetAssignment> {
                new CreateFromBudgetAssignments.Command.BudgetAssignment { Amount = 105.5m, BudgetCategoryId = cat1.Id, BudgetMonth = new Entities.BudgetMonth(2023, 1), Note = "Test note" },
                new CreateFromBudgetAssignments.Command.BudgetAssignment { Amount = 100m, BudgetCategoryId = cat2.Id, BudgetMonth = new Entities.BudgetMonth(2023, 1) }
            },
        };
        var transactionId = await _fixture.SendAsync(command);

        var created = await _fixture.ExecuteDbContextAsync(db => db.AccountTransactions.Where(trans => trans.Id == transactionId).Include(trans => trans.BudgetAssignments).SingleOrDefaultAsync());

        created.ShouldNotBeNull();
        created.Id.ShouldBe(transactionId);
        created.AccountPartition.Id.ShouldBe(partition.Id);
        created.Amount.ShouldBe(205.5m);
        created.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 5));

        created.BudgetAssignments.Count.ShouldBe(2);

        var ba1 = created.BudgetAssignments.ElementAt(0);
        var ba2 = created.BudgetAssignments.ElementAt(1);

        ba1.ShouldNotBeNull();
        ba1.Amount.ShouldBe(105.5m);
        ba1.BudgetCategoryId.ShouldBe(cat1.Id);
        ba1.BudgetMonth.ShouldBeEquivalentTo(new Entities.BudgetMonth(2023, 1));
        ba1.Note.ShouldBe("Test note");

        ba2.ShouldNotBeNull();
        ba2.Amount.ShouldBe(100m);
        ba2.BudgetCategoryId.ShouldBe(cat2.Id);
        ba2.BudgetMonth.ShouldBeEquivalentTo(new Entities.BudgetMonth(2023, 1));
    }
}