using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ching.Entities;

namespace Ching.Data.Config;

public class BudgetIncreaseConfiguration : IEntityTypeConfiguration<BudgetIncrease>
{
    public void Configure(EntityTypeBuilder<BudgetIncrease> builder)
    {
        builder.OwnsOne(a => a.BudgetMonth);
        builder.Navigation(a => a.Transfer).AutoInclude();
        builder.Navigation(a => a.BudgetCategory).AutoInclude();
    }
}