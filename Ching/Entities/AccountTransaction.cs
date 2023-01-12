namespace Ching.Entities;

public class AccountTransaction : BaseEntity
{
    public DateOnly Date { get; private set; }
    public Decimal Amount { get; private set; }
    public string? Note { get; private set; }
    public ICollection<BudgetAssignmentTransaction> BudgetAssignments { get; private set; }

    public int AccountPartitionId { get; private set; }
    public AccountPartition AccountPartition { get; private set; }

    private AccountTransaction() { }
}