using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ching.Entities;

namespace Ching.Data.Config;

public class BudgetAssignmentTransactionConfiguration
    : IEntityTypeConfiguration<BudgetAssignmentTransaction>
{
    public void Configure(EntityTypeBuilder<BudgetAssignmentTransaction> builder)
    {
        builder.OwnsOne(a => a.BudgetMonth);
        builder.Navigation(a => a.BudgetCategory).AutoInclude();
        builder.Navigation(a => a.AccountTransaction).AutoInclude();
    }
}
