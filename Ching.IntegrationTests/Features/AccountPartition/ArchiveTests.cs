using Microsoft.EntityFrameworkCore;
using Ching.Features.AccountPartition;

namespace Ching.IntegrationTests.Features.AccountPartition;

[Collection(nameof(SliceFixture))]
public class ArchiveTests
{
    private readonly SliceFixture _fixture;
    public ArchiveTests(SliceFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_archive_partition()
    {
        var account = await _fixture.FindAsync<Entities.Account>(a => a.Name == "Seed account");

        var command = new Create.Command { Name = "Partition 2 name", AccountId = account.Id };
        var partitionId = await _fixture.SendAsync(command);

        var created = await _fixture.ExecuteDbContextAsync(db => db.AccountPartitions.Where(part => part.Name == command.Name).SingleOrDefaultAsync());

        created.ShouldNotBeNull();
        created.Archived.ShouldBe(false);

        var archiveCommand = new Archive.Command { AccountPartitionId = partitionId };
        await _fixture.SendAsync(archiveCommand);

        var archivedPartition = await _fixture.ExecuteDbContextAsync(db => db.AccountPartitions.Where(part => part.Id == created.Id).SingleOrDefaultAsync());
        archivedPartition.ShouldNotBeNull();
        archivedPartition.Archived.ShouldBeTrue();
    }
}