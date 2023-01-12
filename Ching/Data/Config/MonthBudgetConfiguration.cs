using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ching.Entities;

namespace Ching.Data.Config;

public class MonthBudgetConfiguration : IEntityTypeConfiguration<MonthBudget>
{
    public void Configure(EntityTypeBuilder<MonthBudget> builder)
    {
        builder.OwnsOne(a => a.BudgetMonth);
    }
}