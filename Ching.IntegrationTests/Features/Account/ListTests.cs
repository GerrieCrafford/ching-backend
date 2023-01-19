using Microsoft.EntityFrameworkCore;
using Ching.Features.Account;

namespace Ching.IntegrationTests.Features.Account;

[Collection(nameof(SliceFixture))]
public class ListTests : BaseTest
{
    private readonly SliceFixture _fixture;

    public ListTests(SliceFixture fixture) : base(fixture) => _fixture = fixture;

    [Fact]
    public async Task Should_return_accounts()
    {
        var query = new List.Query { };

        var result = await _fixture.SendAsync(query);

        result.ShouldNotBeNull();
        result.Accounts.Select(x => x.Name).ShouldBe(new List<string> { "ACC1", "ACC2", "ACC3" });
    }
}