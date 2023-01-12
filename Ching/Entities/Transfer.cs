namespace Ching.Entities;

public class Transfer : BaseEntity
{
    public DateOnly Date { get; private set; }
    public Decimal Amount { get; private set; }

    public int SourcePartitionId { get; private set; }
    public AccountPartition SourcePartition { get; private set; }

    public int DestinationPartitionId { get; private set; }
    public AccountPartition DestinationPartition { get; private set; }

    public ICollection<BudgetAssignmentTransfer> BudgetAssignments { get; private set; }

    private Transfer() { }
}