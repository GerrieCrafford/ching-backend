using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Ching.Data;
using Ching.Entities;

namespace Ching.IntegrationTests;

[CollectionDefinition(nameof(SliceFixture))]
public class SliceFixtureCollection : ICollectionFixture<SliceFixture> { }

public class SliceFixture : IAsyncLifetime
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WebApplicationFactory<Program> _factory;

    public SliceFixture()
    {
        _factory = new ChingTestApplicationFactory();

        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    class ChingTestApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ChingContext>));

                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var dbConnection = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbConnection));

                if (dbConnection != null)
                    services.Remove(dbConnection);

                // Create open SqliteConnection so EF won't automatically close it.
                services.AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<ChingContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });
            });

            builder.UseEnvironment("Development");
        }
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChingContext>();

        try
        {
            await dbContext.Database.BeginTransactionAsync();
            var result = await action(scope.ServiceProvider);
            await dbContext.Database.CommitTransactionAsync();
            return result;
        }
        catch (System.Exception)
        {
            dbContext.Database.RollbackTransaction();
            throw;
        }
    }

    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChingContext>();

        try
        {
            await dbContext.Database.BeginTransactionAsync();
            await action(scope.ServiceProvider);
            await dbContext.Database.CommitTransactionAsync();
        }
        catch (System.Exception)
        {
            dbContext.Database.RollbackTransaction();
            throw;
        }
    }

    public Task<T> ExecuteDbContextAsync<T>(Func<ChingContext, Task<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<ChingContext>()!));

    public Task ExecuteDbContextAsync(Func<ChingContext, Task> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<ChingContext>()!));

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            return mediator.Send(request);
        });
    }

    public Task<T?> FindAsync<T>(int id)
        where T : class, IEntity
    {
        return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id).AsTask());
    }

    public Task<T?> FindAsync<T>(Func<T, bool> predicate)
        where T : class, IEntity
    {
        return ExecuteDbContextAsync(db =>
        {
            var res = db.Set<T>().Where(predicate).SingleOrDefault();
            return Task.FromResult(res);
        });
    }

    public Task<T?> GetLast<T>()
        where T : class, IEntity
    {
        return ExecuteDbContextAsync<T?>(db =>
            db.Set<T>().OrderByDescending(x => x.Id).FirstOrDefaultAsync()
        );
    }

    public Task<int> InsertAsync<T>(T entity)
        where T : class, IEntity
    {
        return ExecuteDbContextAsync(async db =>
        {
            db.Set<T>().Add(entity);
            await db.SaveChangesAsync();
            return entity.Id;
        });
    }


    public Task InitializeAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChingContext>();

        dbContext.Database.EnsureCreated();

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _factory?.Dispose();
        return Task.CompletedTask;
    }

    public async Task ClearDatabase(ChingContext db)
    {
        db.Accounts.RemoveRange(db.Accounts);
        db.AccountPartitions.RemoveRange(db.AccountPartitions);
        db.AccountTransactions.RemoveRange(db.AccountTransactions);
        db.BudgetAssignmentsTransactions.RemoveRange(db.BudgetAssignmentsTransactions);
        db.BudgetAssignmentsTransfers.RemoveRange(db.BudgetAssignmentsTransfers);
        db.BudgetCategories.RemoveRange(db.BudgetCategories);
        db.BudgetIncreases.RemoveRange(db.BudgetIncreases);
        db.MonthBudgets.RemoveRange(db.MonthBudgets);
        db.Settlements.RemoveRange(db.Settlements);
        db.Transfers.RemoveRange(db.Transfers);

        // Reset auto-increments
        var tableNames = new string[] { "Accounts", "AccountPartitions", "AccountTransactions", "BudgetAssignmentTransaciton", "BudgetAssignmentTransfers", "BudgetCategories", "BudgetIncreases", "MonthBudgets", "Settlements", "Transfers" };
        foreach (var tableName in tableNames)
        {
            db.Database.ExecuteSqlRaw($"DELETE FROM SQLITE_SEQUENCE WHERE name='{tableName}'");
        }

        await db.SaveChangesAsync();
    }

    public async Task SeedDatabase(ChingContext db)
    {
        // Accounts
        var acc1 = new Entities.Account("ACC1");
        var acc1part1 = new Entities.AccountPartition("ACC1P1");
        var acc1part2 = new Entities.AccountPartition("ACC1P2");
        acc1.Partitions.Add(acc1part1);
        acc1.Partitions.Add(acc1part2);
        await db.Accounts.AddAsync(acc1);

        var acc2 = new Entities.Account("ACC2");
        var acc2part1 = new Entities.AccountPartition("ACC2P1");
        var acc2part2 = new Entities.AccountPartition("ACC2P2");
        acc2.Partitions.Add(acc2part1);
        acc2.Partitions.Add(acc2part2);
        await db.Accounts.AddAsync(acc2);

        var acc3 = new Entities.Account("ACC3");
        var acc3part1 = new Entities.AccountPartition("ACC3P1");
        var acc3part2 = new Entities.AccountPartition("ACC3P2");
        acc3.Partitions.Add(acc3part1);
        acc3.Partitions.Add(acc3part2);
        await db.Accounts.AddAsync(acc3);

        // Categories
        var cat1 = new Entities.BudgetCategory("Seed category 1");
        var cat2 = new Entities.BudgetCategory("Seed category 2");
        var cat3 = new Entities.BudgetCategory("Seed category 3");
        await db.BudgetCategories.AddRangeAsync(cat1, cat2, cat3);

        // Month budgets
        var mb1 = new Entities.MonthBudget(113, new BudgetMonth(2023, 1), cat1);
        var mb2 = new Entities.MonthBudget(403, new BudgetMonth(2023, 1), cat2);
        var mb3 = new Entities.MonthBudget(551, new BudgetMonth(2023, 1), cat3);
        await db.MonthBudgets.AddRangeAsync(mb1, mb2, mb3);

        await db.SaveChangesAsync();
    }
}