using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ching.Entities;

namespace Ching.Data.Config;

public class AccountPartitionConfiguration : IEntityTypeConfiguration<AccountPartition>
{
    public void Configure(EntityTypeBuilder<AccountPartition> builder)
    {
        builder.OwnsOne(a => a.BudgetMonth);
    }
}