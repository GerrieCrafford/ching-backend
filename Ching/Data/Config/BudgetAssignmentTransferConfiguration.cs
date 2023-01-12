using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ching.Entities;

namespace Ching.Data.Config;

public class BudgetAssignmentTransferConfiguration : IEntityTypeConfiguration<BudgetAssignmentTransfer>
{
    public void Configure(EntityTypeBuilder<BudgetAssignmentTransfer> builder)
    {
        builder.OwnsOne(a => a.BudgetMonth);
    }
}