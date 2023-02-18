using Microsoft.EntityFrameworkCore;
using Ching.Entities;
using Ching.Features.BudgetAssignment;
using CreateFromBudgetAssignments = Ching.Features.AccountTransaction.CreateFromBudgetAssignments;
using CreateSavingsPayment = Ching.Features.Transfer.CreateSavingsPayment;
using Ching.DTOs;

namespace Ching.IntegrationTests.Features.BudgetAssignment;

[Collection(nameof(SliceFixture))]
public class ListTests : BaseTest
{
    private readonly SliceFixture _fixture;

    public ListTests(SliceFixture fixture)
        : base(fixture) => _fixture = fixture;

    private List<int> _expectedAssignmentIds = new List<int>();
    private int _cat1Id;
    private int _cat2Id;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var cat1 = await _fixture.FindAsync<Entities.BudgetCategory>(
            cat => cat.Name == "Seed category 1"
        );
        var cat2 = await _fixture.FindAsync<Entities.BudgetCategory>(
            cat => cat.Name == "Seed category 2"
        );

        var part1 = await _fixture.FindAsync<Entities.AccountPartition>(
            part => part.Name == "ACC1P1"
        );
        var part2 = await _fixture.FindAsync<Entities.AccountPartition>(
            part => part.Name == "ACC1P2"
        );

        cat1.ShouldNotBeNull();
        cat2.ShouldNotBeNull();
        part1.ShouldNotBeNull();
        part2.ShouldNotBeNull();

        var accountTransaction1Id = await _fixture.SendAsync(
            new CreateFromBudgetAssignments.Command(
                part1.Id,
                new DateOnly(2023, 2, 3),
                "Some recipient",
                new List<CreateFromBudgetAssignments.Command.BudgetAssignment>
                {
                    new CreateFromBudgetAssignments.Command.BudgetAssignment(
                        cat1.Id,
                        new BudgetMonthDTO(2023, 2),
                        10m
                    ),
                    new CreateFromBudgetAssignments.Command.BudgetAssignment(
                        cat2.Id,
                        new BudgetMonthDTO(2023, 2),
                        15m
                    ),
                }
            )
        );
        var accountTransaction2Id = await _fixture.SendAsync(
            new CreateFromBudgetAssignments.Command(
                part1.Id,
                new DateOnly(2023, 2, 20),
                "Some other recipient",
                new List<CreateFromBudgetAssignments.Command.BudgetAssignment>
                {
                    new CreateFromBudgetAssignments.Command.BudgetAssignment(
                        cat1.Id,
                        new BudgetMonthDTO(2023, 2),
                        30m
                    ),
                    new CreateFromBudgetAssignments.Command.BudgetAssignment(
                        cat2.Id,
                        new BudgetMonthDTO(2023, 2),
                        55m
                    ),
                }
            )
        );

        var transfer1Id = await _fixture.SendAsync(
            new CreateSavingsPayment.Command(
                new DateOnly(2023, 2, 15),
                part1.Id,
                part2.Id,
                new CreateSavingsPayment.Command.BudgetAssignmentData(
                    cat1.Id,
                    new BudgetMonthDTO(2023, 2),
                    150m
                )
            )
        );

        var transfer2Id = await _fixture.SendAsync(
            new CreateSavingsPayment.Command(
                new DateOnly(2023, 2, 11),
                part2.Id,
                part1.Id,
                new CreateSavingsPayment.Command.BudgetAssignmentData(
                    cat2.Id,
                    new BudgetMonthDTO(2023, 2),
                    90m
                )
            )
        );

        var (assignment1Id, assignment2Id, assignment3Id) =
            await _fixture.ExecuteDbContextAsync(async db =>
            {
                var cat1 = await db.BudgetCategories
                    .Where(x => x.Name == "Seed category 1")
                    .FirstAsync();
                var cat2 = await db.BudgetCategories
                    .Where(x => x.Name == "Seed category 2")
                    .FirstAsync();

                var at1 = await db.AccountTransactions
                    .Include(a => a.BudgetAssignments)
                    .FirstAsync(x => x.Id == accountTransaction1Id);
                var at2 = await db.AccountTransactions
                    .Include(a => a.BudgetAssignments)
                    .FirstAsync(x => x.Id == accountTransaction2Id);
                var sp1 = await db.Transfers
                    .Include(t => t.BudgetAssignment)
                    .FirstAsync(x => x.Id == transfer1Id);

                var assignment1Id = at1.BudgetAssignments.First().Id;
                var assignment2Id = at2.BudgetAssignments.First().Id;
                var assignment3Id = sp1.BudgetAssignment?.Id ?? -1;

                return (assignment1Id, assignment2Id, assignment3Id);
            });

        // Expected order is different due to sorting
        _expectedAssignmentIds = new List<int> { assignment2Id, assignment3Id, assignment1Id };
        _cat1Id = cat1.Id;
        _cat2Id = cat2.Id;
    }

    [Fact]
    public async Task Should_return_all_assignments()
    {
        var query = new List.Query();
        var assignments = await _fixture.SendAsync(query);

        assignments.ShouldNotBeNull();

        assignments.BudgetAssignments.Count.ShouldBe(6);
        assignments.BudgetAssignments
            .Select(ba => ba.BudgetCategoryId)
            .Distinct()
            .ToList()
            .ShouldBeEquivalentTo(new List<int> { _cat1Id, _cat2Id });
    }

    [Fact]
    public async Task Should_return_specific_category_assignments()
    {
        var query = new List.Query(_cat1Id);
        var assignments = await _fixture.SendAsync(query);

        assignments.ShouldNotBeNull();

        assignments.BudgetAssignments.Count.ShouldBe(3);
        assignments.BudgetAssignments.ShouldAllBe(x => x.BudgetCategoryId == _cat1Id);
        assignments.BudgetAssignments.Select(x => x.Id).ShouldBe(_expectedAssignmentIds);
        assignments.BudgetAssignments
            .Select(x => x.Type)
            .ShouldBe(
                new List<BudgetAssignmentType>
                {
                    BudgetAssignmentType.Transaction,
                    BudgetAssignmentType.Transfer,
                    BudgetAssignmentType.Transaction,
                }
            );
        assignments.BudgetAssignments.Select(x => x.Amount).ShouldBe(new[] { 30m, 150m, 10m });
        assignments.BudgetAssignments
            .Select(x => x.Recipient)
            .ShouldBe(new[] { "Some other recipient", "Internal", "Some recipient" });
        assignments.BudgetAssignments
            .Select(x => x.Date)
            .ShouldBe(
                new[]
                {
                    new DateOnly(2023, 2, 20),
                    new DateOnly(2023, 2, 15),
                    new DateOnly(2023, 2, 3),
                }
            );
    }

    [Fact]
    public async Task Should_return_empty_list_if_no_assignments()
    {
        var query = new List.Query(3);
        var assignments = await _fixture.SendAsync(query);

        assignments.ShouldNotBeNull();
        assignments.BudgetAssignments.Count.ShouldBe(0);
    }
}
