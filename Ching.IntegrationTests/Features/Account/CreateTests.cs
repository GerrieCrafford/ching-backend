using Microsoft.EntityFrameworkCore;
using Ching.Features.Account;

namespace Ching.IntegrationTests.Features.Account;

[Collection(nameof(SliceFixture))]
public class CreateTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public CreateTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_new_account_with_remaining_partition()
    {
        var command = new Create.Command { Name = "Account name " };

        var accountId = await _fixture.SendAsync(command);

        var created = await _fixture.ExecuteDbContextAsync(db => db.Accounts.Where(acc => acc.Name == command.Name).Include(a => a.Transactions).Include(a => a.Partitions).SingleOrDefaultAsync());

        created.ShouldNotBeNull();
        created.Name.ShouldBe(command.Name);
        created.Transactions.Count.ShouldBe(0);

        created.Partitions.Count.ShouldBe(1);

        var partition = created.Partitions.First();
        partition.ShouldNotBeNull();

        partition.Name.ShouldBe("Remaining");
        partition.Archived.ShouldBe(false);
    }
}