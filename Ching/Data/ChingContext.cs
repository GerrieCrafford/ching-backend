using Microsoft.EntityFrameworkCore;
using Ching.Entities;
using Ching.Data.Config;

namespace Ching.Data;

public class ChingContext : DbContext
{
    public ChingContext(DbContextOptions<ChingContext> options)
    : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountPartition> AccountPartitions => Set<AccountPartition>();
    public DbSet<AccountTransaction> AccountTransactions => Set<AccountTransaction>();
    public DbSet<BudgetAssignmentTransfer> BudgetAssignmentsTransfers => Set<BudgetAssignmentTransfer>();
    public DbSet<BudgetAssignmentTransaction> BudgetAssignmentsTransactions => Set<BudgetAssignmentTransaction>();
    public DbSet<BudgetCategory> BudgetCategories => Set<BudgetCategory>();
    public DbSet<BudgetIncrease> BudgetIncreases => Set<BudgetIncrease>();
    public DbSet<MonthBudget> MonthBudgets => Set<MonthBudget>();
    public DbSet<Settlement> Settlements => Set<Settlement>();
    public DbSet<Transfer> Transfers => Set<Transfer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new AccountPartitionConfiguration().Configure(modelBuilder.Entity<AccountPartition>());
        new AccountTransactionConfiguration().Configure(modelBuilder.Entity<AccountTransaction>());
        new BudgetAssignmentTransferConfiguration().Configure(modelBuilder.Entity<BudgetAssignmentTransfer>());
        new BudgetAssignmentTransactionConfiguration().Configure(modelBuilder.Entity<BudgetAssignmentTransaction>());
        new BudgetIncreaseConfiguration().Configure(modelBuilder.Entity<BudgetIncrease>());
        new MonthBudgetConfiguration().Configure(modelBuilder.Entity<MonthBudget>());
    }
}