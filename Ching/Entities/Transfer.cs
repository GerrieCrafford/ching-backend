namespace Ching.Entities;

public class Transfer : BaseEntity
{
    public DateOnly Date { get; set; }
    public Decimal Amount { get; set; }

    public int SourcePartitionId { get; set; }
    public AccountPartition SourcePartition { get; set; }

    public int DestinationPartitionId { get; set; }
    public AccountPartition DestinationPartition { get; set; }

    public ICollection<BudgetAssignmentTransfer> BudgetAssignments { get; set; }
}