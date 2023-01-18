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
    private readonly IConfiguration _configuration;
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

                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);

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

    public Task<T> ExecuteDbContextAsync<T>(Func<ChingContext, Task<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<ChingContext>()));

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


    public async Task InitializeAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ChingContext>();

        dbContext.Database.EnsureCreated();
        await SeedDatabase(dbContext);
    }

    public Task DisposeAsync()
    {
        _factory?.Dispose();
        return Task.CompletedTask;
    }

    public async Task SeedDatabase(ChingContext db)
    {
        var acc = new Entities.Account("Seed account");
        var partition1 = new Entities.AccountPartition("Seed partition 1");
        var partition2 = new Entities.AccountPartition("Seed partition 2");
        acc.Partitions.Add(partition1);
        acc.Partitions.Add(partition2);
        await db.Accounts.AddAsync(acc);

        var cat1 = new Entities.BudgetCategory("Seed category 1");
        var cat2 = new Entities.BudgetCategory("Seed category 2");
        await db.BudgetCategories.AddAsync(cat1);
        await db.BudgetCategories.AddAsync(cat2);

        await db.SaveChangesAsync();
    }
}