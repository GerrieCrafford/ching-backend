using Microsoft.EntityFrameworkCore;
using Ching.Features.AccountPartition;

namespace Ching.IntegrationTests.Features.AccountPartition;

[Collection(nameof(SliceFixture))]
public class ArchiveTests : BaseTest
{
    private readonly SliceFixture _fixture;
    public ArchiveTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_archive_partition()
    {
        var account = await _fixture.FindAsync<Entities.Account>(a => a.Name == "ACC1");
        account.ShouldNotBeNull();

        var command = new Create.Command(account.Id, "Partition 2 name");
        var partitionId = await _fixture.SendAsync(command);

        var created = await _fixture.ExecuteDbContextAsync(db => db.AccountPartitions.Where(part => part.Name == command.Name).SingleOrDefaultAsync());

        created.ShouldNotBeNull();
        created.Archived.ShouldBe(false);

        var archiveCommand = new Archive.Command(partitionId);
        await _fixture.SendAsync(archiveCommand);

        var archivedPartition = await _fixture.ExecuteDbContextAsync(db => db.AccountPartitions.Where(part => part.Id == created.Id).SingleOrDefaultAsync());
        archivedPartition.ShouldNotBeNull();
        archivedPartition.Archived.ShouldBeTrue();
    }
}