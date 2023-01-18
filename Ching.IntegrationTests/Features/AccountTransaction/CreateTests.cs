using Microsoft.EntityFrameworkCore;
using Ching.Features.AccountTransaction;

namespace Ching.IntegrationTests.Features.AccountTransaction;

[Collection(nameof(SliceFixture))]
public class CreateTests
{
    private readonly SliceFixture _fixture;
    public CreateTests(SliceFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_new_account_transaction()
    {
        var account = await _fixture.FindAsync<Entities.Account>(a => a.Name == "Seed account");
        var partition = await _fixture.FindAsync<Entities.AccountPartition>(p => p.Name == "Seed partition 1");
        var cat1 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 1");
        var cat2 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 2");

        var command = new Create.Command
        {
            AccountPartitionId = partition.Id,
            Date = new DateOnly(2023, 1, 5),
            BudgetAssignments = new List<Create.Command.BudgetAssignment> {
                new Create.Command.BudgetAssignment { Amount = 105.5m, BudgetCategoryId = cat1.Id, BudgetMonth = new Entities.BudgetMonth(2023, 1) },
                new Create.Command.BudgetAssignment { Amount = 100m, BudgetCategoryId = cat2.Id, BudgetMonth = new Entities.BudgetMonth(2023, 1) }
            },
            Note = "Test note"
        };
        var transactionId = await _fixture.SendAsync(command);

        var created = await _fixture.ExecuteDbContextAsync(db => db.AccountTransactions.Where(trans => trans.Id == transactionId).Include(trans => trans.BudgetAssignments).SingleOrDefaultAsync());

        created.ShouldNotBeNull();
        created.Id.ShouldBe(transactionId);
        created.AccountPartition.Id.ShouldBe(partition.Id);
        created.Amount.ShouldBe(205.5m);
        created.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 5));
        created.Note.ShouldBe("Test note");

        created.BudgetAssignments.Count.ShouldBe(2);

        var ba1 = created.BudgetAssignments.ElementAt(0);
        var ba2 = created.BudgetAssignments.ElementAt(1);

        ba1.ShouldNotBeNull();
        ba1.Amount.ShouldBe(105.5m);
        ba1.BudgetCategoryId.ShouldBe(cat1.Id);
        ba1.BudgetMonth.ShouldBeEquivalentTo(new Entities.BudgetMonth(2023, 1));

        ba2.ShouldNotBeNull();
        ba2.Amount.ShouldBe(100m);
        ba2.BudgetCategoryId.ShouldBe(cat2.Id);
        ba2.BudgetMonth.ShouldBeEquivalentTo(new Entities.BudgetMonth(2023, 1));
    }
}