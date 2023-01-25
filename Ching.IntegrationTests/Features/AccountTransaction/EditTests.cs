using Microsoft.EntityFrameworkCore;
using Ching.Features.AccountTransaction;

namespace Ching.IntegrationTests.Features.AccountTransaction;

[Collection(nameof(SliceFixture))]
public class EditTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public EditTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_update_account_transaction()
    {
        var account = await _fixture.FindAsync<Entities.Account>(a => a.Name == "ACC1");
        var partition = await _fixture.FindAsync<Entities.AccountPartition>(p => p.Name == "ACC1P1");
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
        };
        var transactionId = await _fixture.SendAsync(command);

        var editCommand = new Edit.Command
        {
            AccountTransactionId = transactionId,
            Date = new DateOnly(2023, 3, 6),
            BudgetAssignments = new List<Edit.Command.BudgetAssignment> {
                new Edit.Command.BudgetAssignment { Amount = 205.5m, BudgetCategoryId = cat2.Id, BudgetMonth = new Entities.BudgetMonth(2023, 2) },
            },
            Note = "Test note"
        };
        await _fixture.SendAsync(editCommand);

        var edited = await _fixture.ExecuteDbContextAsync(db => db.AccountTransactions.Where(trans => trans.Id == transactionId).Include(x => x.BudgetAssignments).SingleOrDefaultAsync());

        edited.ShouldNotBeNull();
        edited.Date.ShouldBeEquivalentTo(new DateOnly(2023, 3, 6));
        edited.Note.ShouldBe("Test note");
        edited.BudgetAssignments.Count.ShouldBe(1);
        edited.BudgetAssignments[0].Amount.ShouldBe(205.5m);
        edited.BudgetAssignments[0].BudgetCategory.Id.ShouldBe(cat2.Id);
        edited.BudgetAssignments[0].BudgetMonth.Year.ShouldBe(2023);
        edited.BudgetAssignments[0].BudgetMonth.Month.ShouldBe(2);
    }
}