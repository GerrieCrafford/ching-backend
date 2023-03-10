using Microsoft.EntityFrameworkCore;
using Ching.Features.AccountPartition;

namespace Ching.IntegrationTests.Features.AccountPartition;

[Collection(nameof(SliceFixture))]
public class CreateTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public CreateTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_create_new_account_partition()
    {
        var account = await _fixture.FindAsync<Entities.Account>(a => a.Name == "ACC1");
        account.ShouldNotBeNull();

        var command = new Create.Command(account.Id, "Partition name");
        var partitionId = await _fixture.SendAsync(command);

        var created = await _fixture.ExecuteDbContextAsync(db => db.AccountPartitions.Where(part => part.Name == command.Name).SingleOrDefaultAsync());

        created.ShouldNotBeNull();
        created.Id.ShouldBe(partitionId);
        created.Name.ShouldBe(command.Name);
        created.Archived.ShouldBe(false);
        created.Account.Id.ShouldBe(account.Id);
    }
}