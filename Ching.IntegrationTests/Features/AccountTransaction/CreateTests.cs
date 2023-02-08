using Microsoft.EntityFrameworkCore;
using Ching.Features.AccountTransaction;

namespace Ching.IntegrationTests.Features.AccountTransaction;

[Collection(nameof(SliceFixture))]
public class CreateTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public CreateTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_new_account_transaction()
    {
        var account = await _fixture.FindAsync<Entities.Account>(a => a.Name == "ACC1");
        var partition = await _fixture.FindAsync<Entities.AccountPartition>(p => p.Name == "ACC1P1");
        var cat1 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 1");
        var cat2 = await _fixture.FindAsync<Entities.BudgetCategory>(x => x.Name == "Seed category 2");

        partition.ShouldNotBeNull();

        var command = new Create.Command
        (
            partition.Id,
            new DateOnly(2023, 1, 5),
            13.5m,
            "Recipient test",
            "Some note"
        );
        var transactionId = await _fixture.SendAsync(command);

        var created = await _fixture.ExecuteDbContextAsync(db => db.AccountTransactions.Where(trans => trans.Id == transactionId).SingleOrDefaultAsync());

        created.ShouldNotBeNull();
        created.Id.ShouldBe(transactionId);
        created.AccountPartition.Id.ShouldBe(partition.Id);
        created.Amount.ShouldBe(13.5m);
        created.Date.ShouldBeEquivalentTo(new DateOnly(2023, 1, 5));
        created.Recipient.ShouldBe("Recipient test");
        created.Note.ShouldBe("Some note");
    }
}