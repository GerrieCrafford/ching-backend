using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ching.Entities;

namespace Ching.Data.Config;

public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        builder.Navigation(a => a.SourcePartition).AutoInclude();
        builder.Navigation(a => a.DestinationPartition).AutoInclude();
        builder.Navigation(a => a.BudgetAssignment).AutoInclude();
    }
}