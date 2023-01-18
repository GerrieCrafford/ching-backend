namespace Ching.Entities;

public class Transfer : BaseEntity
{
    public DateOnly Date { get; set; }
    public Decimal Amount { get; set; }

    public int SourcePartitionId { get; set; }
    public AccountPartition SourcePartition { get; set; }

    public int DestinationPartitionId { get; set; }
    public AccountPartition DestinationPartition { get; set; }

    public List<BudgetAssignmentTransfer> BudgetAssignments { get; set; }

    private Transfer() { }

    public Transfer(DateOnly date, decimal amount, AccountPartition source, AccountPartition dest, List<BudgetAssignmentTransfer>? budgetAssignments)
    {
        Date = date;
        Amount = amount;
        SourcePartition = source;
        DestinationPartition = dest;
        BudgetAssignments = budgetAssignments ?? new List<BudgetAssignmentTransfer>();
    }

    public Transfer(DateOnly date, decimal amount, AccountPartition source, AccountPartition dest)
    : this(date, amount, source, dest, null) { }
}