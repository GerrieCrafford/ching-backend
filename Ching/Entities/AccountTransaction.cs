namespace Ching.Entities;

public class AccountTransaction : BaseEntity
{
    public DateOnly Date { get; set; }
    public Decimal Amount { get; set; }
    public string? Note { get; set; }
    public ICollection<BudgetAssignmentTransaction> BudgetAssignments { get; set; }

    public int AccountPartitionId { get; set; }
    public AccountPartition AccountPartition { get; set; }
}