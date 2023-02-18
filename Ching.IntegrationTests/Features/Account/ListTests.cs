using Microsoft.EntityFrameworkCore;
using AccountEntity = Ching.Entities.Account;
using Ching.Features.Account;

namespace Ching.IntegrationTests.Features.Account;

[Collection(nameof(SliceFixture))]
public class ListTests : BaseTest
{
    private readonly SliceFixture _fixture;

    public ListTests(SliceFixture fixture)
        : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_return_accounts()
    {
        var accountId = await _fixture.ExecuteDbContextAsync(async db =>
        {
            var account = new AccountEntity("Test");
            await db.Accounts.AddAsync(account);
            await db.SaveChangesAsync();
            return account.Id;
        });

        var query = new List.Query { };

        var result = await _fixture.SendAsync(query);

        result.ShouldNotBeNull();
        result.Accounts
            .Select(x => x.Name)
            .ShouldBe(new List<string> { "ACC1", "ACC2", "ACC3", "Test" });
        result.Accounts.Select(x => x.Partitions.Count).ShouldBe(new List<int> { 3, 3, 3, 1 });
    }
}
